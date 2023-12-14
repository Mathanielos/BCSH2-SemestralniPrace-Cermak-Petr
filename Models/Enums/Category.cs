using System.ComponentModel;

namespace BCSH2SemestralniPraceCermakPetr.Models.Enums
{
    public enum Category
    {
        [Description("Všechna místa")]
        VsechnaMista,
        [Description("Historické památky")]
        HistorickePamatky,
        [Description("Přírodní krásy")]
        PrirodniKrasy,
        [Description("Parky")]
        Parky,
        [Description("Náměstí")]
        Namesti,
        [Description("Muzea a galerie")]
        Muzea,
        [Description("Sport a zábava")]
        Sport,
        [Description("Čtvrti/Ulice")]
        Ctvrti,
        [Description("Nákupy a trhy")]
        Nakupy,
        [Description("Pláže")]
        Plaze,
        [Description("Rurální/Městské oblasti")]
        RuralniOblasti
    }
}
