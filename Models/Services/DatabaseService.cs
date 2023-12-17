using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Drawing.Imaging;
using Avalonia.Media.Imaging;
using BCSH2SemestralniPraceCermakPetr.Models.Converters;
using System.Globalization;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;

namespace BCSH2SemestralniPraceCermakPetr.Models.Services
{
    public class DatabaseService
    {
        private string connectionString;
        private string databaseFile;
        public DatabaseService(string databaseFile)
        {
            this.databaseFile = databaseFile;
            connectionString = $"Data Source={databaseFile}";
        }

        public void InitializeDatabase()
        {
            if (!File.Exists(databaseFile))
            {
                SQLiteConnection.CreateFile(databaseFile);
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Countries (CountryID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE, Description TEXT, Tips TEXT, Image BLOB);", connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Cities (CityID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, CountryID INTEGER, Description TEXT, BasicInformation TEXT, Image BLOB, FOREIGN KEY (CountryID) REFERENCES Countries(CountryID));", connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Places (PlaceID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, CityID INTEGER, CategoryID INTEGER, Description TEXT, Image BLOB, FOREIGN KEY (CityID) REFERENCES Cities(CityID), FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID));", connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Categories (CategoryID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE);", connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }


        public int InsertData<T>(T item, int? parentId = null)
        {
            Type itemType = typeof(T);

            string tableName = itemType.GetCustomAttributes(typeof(TableAttribute), true)
                                       .OfType<TableAttribute>()
                                       .FirstOrDefault()?.Name ?? itemType.Name;
            if (tableName == null)
            {
                throw new InvalidOperationException("Table name not found.");
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        List<PropertyInfo> propertiesToInsert = itemType.GetProperties()
                            .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)) && !Attribute.IsDefined(p, typeof(KeyAttribute)))
                            .ToList();

                        // Generate the column names and values for the insert statement
                        string columns = string.Join(", ", propertiesToInsert.Select(p => GetColumnName(p)));
                        string values = string.Join(", ", propertiesToInsert.Select(p => $"@{GetColumnName(p)}"));

                        // Determine the foreign key column based on the type
                        string foreignKeyColumn = "";
                        if (itemType == typeof(Place))
                        {
                            foreignKeyColumn = "CityID";
                        }
                        else if (itemType == typeof(City))
                        {
                            foreignKeyColumn = "CountryID";
                        }

                        // If parentId is not null, add the foreign key column to the columns and values
                        if (parentId.HasValue && !string.IsNullOrEmpty(foreignKeyColumn))
                        {
                            columns += $", {foreignKeyColumn}";
                            values += $", @{foreignKeyColumn}";
                        }

                        // Combine columns and values to form the complete insert statement
                        string insertStatement = $"INSERT INTO {tableName} ({columns}) VALUES ({values});";

                        using (SQLiteCommand command = new SQLiteCommand(insertStatement, connection))
                        {
                            // Set parameters for the insert statement
                            foreach (PropertyInfo property in propertiesToInsert)
                            {
                                object? value = property.GetValue(item);

                                // Handling of special types
                                if (property.PropertyType == typeof(Bitmap) && value != null)
                                {
                                    // Convert Bitmap to byte array (assuming your database expects a byte array)
                                    value = ConvertBitmapToByteArray((Bitmap)value);
                                }
                                else if (property.PropertyType.IsEnum && value != null)
                                {
                                    if (property.PropertyType == typeof(Category))
                                    {
                                        // Convert Category enum to int
                                        value = (int)value;
                                    }
                                    else
                                    {
                                        // Convert enum value to its description
                                        value = ConvertEnumToDescription(value);
                                    }
                                }
                                command.Parameters.AddWithValue($"@{GetColumnName(property)}", value);
                            }

                            // If parentId is not null, set the parameter for the foreign key
                            if (parentId.HasValue && !string.IsNullOrEmpty(foreignKeyColumn))
                            {
                                command.Parameters.AddWithValue($"@{foreignKeyColumn}", parentId.Value);
                            }

                            // Execute the insert statement
                            command.ExecuteNonQuery();
                        }

                        // Get the last inserted row ID
                        int lastInsertedId = Convert.ToInt32(connection.LastInsertRowId);

                        // Commit the transaction if everything is successful
                        transaction.Commit();

                        return lastInsertedId;
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        throw new InvalidOperationException("Error inserting data.", ex);
                    }
                }
            }
        }


        public List<Dictionary<string, object>> GetData(string tableName, string[] columns, string condition = null, Dictionary<string, object> parameters = null)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string columnNames = columns != null && columns.Length > 0 ? string.Join(", ", columns) : "*";
                string conditionClause = string.IsNullOrWhiteSpace(condition) ? "" : $" WHERE {condition}";

                using (SQLiteCommand command = new SQLiteCommand($"SELECT {columnNames} FROM {tableName}{conditionClause};", connection))
                {
                    if (parameters != null)
                    {
                        foreach (var kvp in parameters)
                        {
                            command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                        }
                    }

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                data[reader.GetName(i)] = reader.GetValue(i);
                            }
                            result.Add(data);
                        }
                    }
                }
            }

            return result;
        }
        public void UpdateData<T>(T item)
        {
            Type itemType = typeof(T);

            string tableName = itemType.GetCustomAttributes(typeof(TableAttribute), true)
                                       .OfType<TableAttribute>()
                                       .FirstOrDefault()?.Name ?? itemType.Name;
            if (tableName == null)
            {
                return;
            }

            PropertyInfo? idProperty = itemType.GetProperties()
                                              .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

            if (idProperty == null)
            {
                throw new InvalidOperationException("No property with KeyAttribute found.");
            }

            // Get the ID value
            object? idValue = idProperty.GetValue(item);

            if (idValue == null)
            {
                throw new InvalidOperationException("ID property value is null.");
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        List<PropertyInfo> propertiesToUpdate = itemType.GetProperties()
                            .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)) && !Attribute.IsDefined(p, typeof(KeyAttribute)))
                            .ToList();

                        // Generate the SET clause for the update statement
                        string setClause = string.Join(", ",
                            propertiesToUpdate.Select(p => $"{GetColumnName(p)} = @{GetColumnName(p)}"));

                        // Generate the WHERE clause for the update statement
                        string conditionClause = $"WHERE {GetColumnName(idProperty)} = @{GetColumnName(idProperty)}";

                        // Combine clauses to form the complete update statement
                        string updateStatement = $"UPDATE {tableName} SET {setClause} {conditionClause};";

                        using (SQLiteCommand command = new SQLiteCommand(updateStatement, connection))
                        {
                            // Set parameters for the update statement
                            foreach (PropertyInfo property in propertiesToUpdate)
                            {
                                object? value = property.GetValue(item);

                                // Handling of special types
                                if (property.PropertyType == typeof(Bitmap) && value != null)
                                {
                                    // Convert Bitmap to byte array (assuming your database expects a byte array)
                                    value = ConvertBitmapToByteArray((Bitmap)value);
                                }
                                else if (property.PropertyType.IsEnum && value != null)
                                {
                                    if (property.PropertyType == typeof(Category))
                                    {
                                        // Convert Category enum to int
                                        value = (int)value;
                                    }
                                    else
                                    {
                                        // Convert enum value to its description
                                        value = ConvertEnumToDescription(value);
                                    }
                                }
                                command.Parameters.AddWithValue($"@{GetColumnName(property)}", value);
                            }
                            command.Parameters.AddWithValue($"@{GetColumnName(idProperty)}", idValue);

                            // Execute the update statement
                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything is successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        throw new InvalidOperationException("Error updating data.", ex);
                    }
                }
            }
        }
        // Helper method to Get name of the column specified in ColumnAttribute
        private string GetColumnName(PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttribute?.Name ?? property.Name;
        }
        // Helper method to convert Bitmap to byte array
        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream);
                return stream.ToArray();
            }
        }
        // Helper method to convert enum to its description
        private string ConvertEnumToDescription(object enumValue)
        {
            EnumDescriptionConverter converter = new EnumDescriptionConverter();
            return (string)converter.Convert(enumValue, typeof(string), null, CultureInfo.CurrentCulture);
        }


        public void DeleteData<T>(T item)
        {
            Type itemType = typeof(T);

            string tableName = itemType.GetCustomAttributes(typeof(TableAttribute), true)
                                       .OfType<TableAttribute>()
                                       .FirstOrDefault()?.Name ?? itemType.Name;
            if (tableName == null)
            {
                return;
            }

            PropertyInfo? idProperty = itemType.GetProperties()
                                              .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

            if (idProperty == null)
            {
                throw new InvalidOperationException("No property with KeyAttribute found.");
            }

            // Get the ID value
            object? idValue = idProperty.GetValue(item);

            if (idValue == null)
            {
                throw new InvalidOperationException("ID property value is null.");
            }

            // Get the Column attribute, if applied
            ColumnAttribute? columnAttribute = idProperty.GetCustomAttribute<ColumnAttribute>();

            // Get the column name, use the property name if the attribute is not applied
            string idColumnName = columnAttribute?.Name ?? idProperty.Name;

            // Use a transaction for the DELETE operation
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string conditionClause = $"WHERE {idColumnName} = @{idColumnName}";

                        using (SQLiteCommand command = new SQLiteCommand($"DELETE FROM {tableName} {conditionClause};", connection))
                        {
                            command.Parameters.AddWithValue($"@{idColumnName}", idValue);
                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything is successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        throw new InvalidOperationException("Error deleting data.", ex);
                    }
                }
            }
        }



    }
}
