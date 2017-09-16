using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using Xunit;

namespace TouhouSplits.UnitTests.Service.Config
{
    public class GameConfigTests
    {
        private const string VALID_CONFIG = @"<Game name=""Some game name"" process=""process1|process2|"">
  <Hook address = ""0x0069BCA4"" encoding=""int32""/>
</Game>";

        [Fact]
        private void IsGameNameSet()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Some game name", config.GameName);
        }
    }
}
