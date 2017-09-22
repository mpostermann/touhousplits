using System.Collections.Generic;
using System.ComponentModel;

namespace TouhouSplits.UnitTests.Utils
{
    public class NotifyPropertyChangedCatcher
    {
        public List<string> CaughtProperties { get; private set; }

        public PropertyChangedEventHandler CatchPropertyChangedEvents {
            get {
                return delegate (object sender, PropertyChangedEventArgs e)
                {
                    CaughtProperties.Add(e.PropertyName);
                };
            }
        }

        public NotifyPropertyChangedCatcher()
        {
            CaughtProperties = new List<string>();
        }
    }
}
