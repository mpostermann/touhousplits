using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Serialization;
using Xunit;

namespace TouhouSplits.IntegrationTests.Service.Serialization
{
    public class JsonSerializerTests
    {
        [Fact]
        public void Serialized_Splits_Is_Deserialized_With_Same_Values()
        {
            string testfilepath = "Serialized_Splits_Is_Deserialized_With_Same_Values.json";

            var splits = new Splits();
            splits.GameName = "Some game name";
            splits.SplitName = "Some split name";
            splits.AddSegment(0, new Segment() {
                SegmentName = "Segment 0",
                Score = 1001
            });
            splits.AddSegment(1, new Segment() {
                SegmentName = "Segment 1",
                Score = 2002
            });
            var serializer = new JsonSerializer<Splits>();

            Splits deserializedSplits;
            try {
                serializer.Serialize(splits, testfilepath);
                deserializedSplits = serializer.Deserialize(testfilepath);
            }
            catch (Exception e) {
                File.Delete(testfilepath);
                throw;
            }
            finally {
                File.Delete(testfilepath);
            }

            Assert.Equal("Some game name", deserializedSplits.GameName);
            Assert.Equal("Some split name", deserializedSplits.SplitName);
            Assert.Equal(2, deserializedSplits.Segments.Count);
            Assert.Equal("Segment 0", deserializedSplits.Segments[0].SegmentName);
            Assert.Equal(1001, deserializedSplits.Segments[0].Score);
            Assert.Equal("Segment 1", deserializedSplits.Segments[1].SegmentName);
            Assert.Equal(2002, deserializedSplits.Segments[1].Score);
            Assert.Equal("Segment 1", deserializedSplits.EndingSegment.SegmentName);
            Assert.Equal(2002, deserializedSplits.EndingSegment.Score);
        }

        [Fact]
        public void Serialized_String_List_Is_Deserialized_With_Save_Values()
        {
            string testfilepath = "Serialized_String_List_Is_Deserialized_With_Save_Values.json";

            var list = new List<string>();
            list.Add("Value 0");
            list.Add("Value 1");
            list.Add("Value 2");
            var serializer = new JsonSerializer<List<string>>();

            List<string> deserializedList;
            try {
                serializer.Serialize(list, testfilepath);
                deserializedList = serializer.Deserialize(testfilepath);
            }
            catch (Exception e) {
                File.Delete(testfilepath);
                throw;
            }
            finally {
                File.Delete(testfilepath);
            }

            Assert.Equal("Value 0", deserializedList[0]);
            Assert.Equal("Value 1", deserializedList[1]);
            Assert.Equal("Value 2", deserializedList[2]);
        }
    }
}
