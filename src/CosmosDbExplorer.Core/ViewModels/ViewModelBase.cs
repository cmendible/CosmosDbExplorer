using System;
using System.Linq.Expressions;
using Prism.Events;
using Prism.Mvvm;

namespace CosmosDbExplorer.ViewModels
{
    public class BindableBaseEx: BindableBase
    {
        protected void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propertyName = memberExpression.Member.Name;
            base.RaisePropertyChanged(propertyName);
        }
    }

    public class ViewModelBase : BindableBaseEx
    {
        public IEventAggregator MessengerInstance { get; }
        
        public ViewModelBase(IEventAggregator messenger = null)
        {
            MessengerInstance = messenger;
        }

        public virtual void Cleanup()
        {

        }
    }
}
