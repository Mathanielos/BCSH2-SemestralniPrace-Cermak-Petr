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
    }
}