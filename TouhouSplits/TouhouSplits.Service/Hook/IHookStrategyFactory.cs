using System.Xml.Linq;

namespace TouhouSplits.Service.Hook
{
    public interface IHookStrategyFactory
    {
        IHookStrategy Create(XElement configElement);
    }
}
