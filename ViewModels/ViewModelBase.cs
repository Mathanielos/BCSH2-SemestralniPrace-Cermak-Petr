using BCSH2SemestralniPraceCermakPetr.Models;
using ReactiveUI;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected ViewModelBase Parent { get; set; }

        public virtual void SetParent(ViewModelBase parent)
        {
            Parent = parent;
        }
        public virtual void RemovePlace(Place place)
        {

        }
        public virtual void RemoveCity(City city)
        {

        }
        public virtual void UpdatePlace(Place place)
        {

        }
        public virtual void UpdateCity(City city)
        {

        }
        public virtual void UpdateCountry(Country country)
        {

        }
        public virtual void InsertPlace(Place place, int parentId)
        {

        }
        public virtual void InsertCity(City city, int parentId)
        {

        }
    }
}