
using System;


namespace Virion
{
    /// <summary>
    /// Our game's implementation of IViewFactory which can handle creating the views
    /// when resuming from being tombstoned.
    /// </summary>
    public class ViewFactory
    {
        public GameView CreateView(Type viewType)
        {
            // All of our views have empty constructors so we can just use Activator
            return Activator.CreateInstance(viewType) as GameView;

            // If we had more complex views that had constructors or needed properties set,
            // we could do that before handing the view back to the ViewManager. For example
            // you might have something like this:
            //
            // if (viewType == typeof(MySuperGameView))
            // {
            //     bool value = GetFirstParameter();
            //     float value2 = GetSecondParameter();
            //     MySuperGameView view = new MySuperGameView(value, value2);
            //     return view;
            // }
            //
            // This lets you still take advantage of constructor arguments yet participate in the
            // serialization process of the view manager. Of course you need to save out those
            // values when deactivating and read them back, but that means either IsolatedStorage or
            // using the PhoneApplicationService.Current.State dictionary.
        }
    }
}
