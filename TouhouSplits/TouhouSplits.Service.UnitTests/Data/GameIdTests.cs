using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class GameIdTests
    {
        [Fact]
        public void ID_Returns_Empty_String_If_Null_Is_Passed_In_Constructor()
        {
            var id = new GameId(null);
            Assert.Equal(string.Empty, id.Id);
        }

        [Fact]
        public void ID_Returns_String_Set_In_Constructor()
        {
            var id = new GameId("Some id");
            Assert.Equal("Some id", id.Id);
        }

        [Fact]
        public void Equals_Returns_True_For_GameId_With_Equal_Id()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Some id");
            Assert.True(id.Equals(otherId));
        }

        [Fact]
        public void Equals_Returns_False_For_GameId_With_Different_Id()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Another id");
            Assert.False(id.Equals(otherId));
        }

        [Fact]
        public void Equals_Returns_False_For_String_Types()
        {
            var id = new GameId("Some id");
            string idString = "Some id";
            Assert.False(id.Equals(idString));
        }

        [Fact]
        public void EqualityComparison_Returns_True_For_GameIds_With_Equal_Ids()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Some id");
            Assert.True(id == otherId);
        }

        [Fact]
        public void EqualityComparison_Returns_False_For_GameIds_With_Different_Ids()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Another id");
            Assert.False(id == otherId);
        }

        [Fact]
        public void InequalityComparison_Returns_True_For_GameIds_With_Different_Ids()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Another id");
            Assert.True(id != otherId);
        }

        [Fact]
        public void InequalityComparison_Returns_False_For_GameIds_With_Equal_Ids()
        {
            var id = new GameId("Some id");
            var otherId = new GameId("Some id");
            Assert.False(id != otherId);
        }
    }
}
