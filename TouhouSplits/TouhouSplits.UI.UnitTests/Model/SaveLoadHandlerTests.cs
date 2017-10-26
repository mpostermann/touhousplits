using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Model
{
    public class SaveLoadHandlerTests
    {
        private static IFileHandler<ISplits> FakeSplitsFile(string filepath)
        {
            var file = Substitute.For<IFileHandler<ISplits>>();
            file.FileInfo.Returns(new FileInfo(filepath));
            return file;
        }

        [Fact]
        public void HasUnsavedChanges_Is_False_By_Default()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            Assert.False(model.HasUnsavedChanges);
        }

        [Fact]
        public void FileName_Is_EmptyString_By_Default()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            Assert.Equal(string.Empty, model.FileName);
        }

        [Fact]
        public void FileName_Returns_Name_Of_CurrentFile()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("somepath/SomeFile.tsf"));
            Assert.Equal("SomeFile.tsf", model.FileName);
        }

        [Fact]
        public void LoadSplitsFile_Sets_CurrentFile_To_Passed_In_SplitsFile()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var file = FakeSplitsFile("SomeFile.tsf");
            model.LoadSplitsFile(file);
            Assert.Equal(file, model.CurrentFile());
        }
        
        [Fact]
        public void LoadSplitsFile_Loads_Passed_In_SplitsFile()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var fileMock = FakeSplitsFile("SomeFile.tsf");
            model.LoadSplitsFile(fileMock);
            var t = fileMock.Received().Object;
        }

        [Fact]
        public void LoadSplitsFile_Closes_CurrentFile_Before_Setting_New_SplitsFile()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var origFileMock = FakeSplitsFile("OrigFile.tsf");
            var newFile = FakeSplitsFile("NewFile.tsf");

            model.LoadSplitsFile(origFileMock);
            model.LoadSplitsFile(newFile);
            origFileMock.Received().Close();
        }

        [Fact]
        public void LoadSplitsFile_Fires_NotifyPropertyChanged_Event_For_FileName()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));
            Assert.True(eventCatcher.CaughtProperties.Contains("FileName"));
        }

        [Fact]
        public void SaveToCurrentFile_Throws_Exception_If_CurrentFile_Is_Not_Set()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            string error;
            Assert.Throws<InvalidOperationException>(() => model.SaveToCurrentFile(Substitute.For<ISplits>(), out error));
        }

        [Fact]
        public void SaveToCurrentFile_Sets_CurrentFile_To_File_Produced_From_SplitsFacade()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splits = Substitute.For<ISplits>();
            var expectedSplitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(splits).Returns(expectedSplitsFile);

            string error;
            model.SaveToCurrentFile(splits, out error);
            Assert.Equal(expectedSplitsFile, model.CurrentFile());
        }

        [Fact]
        public void SaveToCurrentFile_Sets_CurrentFile_FileInfo_To_Previous_FileInfo()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("somepath/SomeFile.tsf"));

            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.Equal(new FileInfo("somepath/SomeFile.tsf").FullName, model.CurrentFile().FileInfo.FullName);
        }

        [Fact]
        public void SaveToCurrentFile_Returns_True_If_Save_Succeeds()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            string error;
            bool result = model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.True(result);
        }

        [Fact]
        public void SaveToCurrentFile_Returns_No_Error_If_Save_Succeeds()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void SaveToCurrentFile_Returns_False_If_Save_Fails()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            splitsFile.When(n => n.Save()).Throw(new Exception());
            string error;
            bool result = model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.False(result);
        }

        [Fact]
        public void SaveToCurrentFile_Returns_Error_If_Save_Fails()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            splitsFile.When(n => n.Save()).Throw(new Exception("Some error message"));
            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.NotNull(error);
            Assert.NotEqual(string.Empty, error);
        }

        [Fact]
        public void SaveToCurrentFile_Sets_HasUnsavedChanges_To_True_If_Save_Fails()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            splitsFile.When(n => n.Save()).Throw(new Exception());
            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.True(model.HasUnsavedChanges);
        }

        [Fact]
        public void SaveToCurrentFile_Sets_HasUnsavedChanged_To_False_If_Save_Succeeds()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            splitsFile.When(n => n.Save()).Throw(new Exception());
            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);

            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(Substitute.For<IFileHandler<ISplits>>());
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);
            Assert.False(model.HasUnsavedChanges);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Throws_Exception_If_CurrentFile_Is_Not_Loaded()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());

            string error;
            Assert.Throws<InvalidOperationException>(() => model.SaveCurrentSplitsAs(new FileInfo("SomeFile.tsf"), out error));
        }

        [Fact]
        public void SaveCurrentSplitsAs_Updates_CurrentFile_FileInfo()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("SomeFile.tsf"));

            string error;
            model.SaveCurrentSplitsAs(new FileInfo("newpath/NewFile.tsf"), out error);
            Assert.Equal(new FileInfo("newpath/NewFile.tsf").FullName, model.CurrentFile().FileInfo.FullName);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Saves_CurrentFile()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var fileMock = FakeSplitsFile("SomeFile.tsf");
            model.LoadSplitsFile(fileMock);

            string error;
            model.SaveCurrentSplitsAs(new FileInfo("NewFile.tsf"), out error);
            fileMock.Received().Save();
        }

        [Fact]
        public void SaveCurrentSplitsAs_Sets_HasUnsavedChanges_To_False_If_Save_Succeeds()
        {
            var splitsFacade = Substitute.For<ISplitsFacade>();
            var model = new SaveLoadHandler(splitsFacade);
            model.LoadSplitsFile(FakeSplitsFile("BadPath.tsf"));

            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFacade.NewSplitsFile(Arg.Any<ISplits>()).Returns(splitsFile);

            splitsFile
                .When(n => n.Save())
                .Do(n => { if (splitsFile.FileInfo.Name == "BadPath.tsf") throw new Exception(); });
            string error;
            model.SaveToCurrentFile(Substitute.For<ISplits>(), out error);

            model.SaveCurrentSplitsAs(new FileInfo("NewFile.tsf"), out error);
            Assert.False(model.HasUnsavedChanges);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Returns_True_If_Save_Succeeds()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("BadPath.tsf"));

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("NewFile.tsf"), out error);
            Assert.True(result);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Returns_False_If_Save_Fails()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var splitsFile = FakeSplitsFile("SomePath.tsf");
            model.LoadSplitsFile(splitsFile);

            splitsFile
                .When(n => n.Save())
                .Do(n => { if (splitsFile.FileInfo.Name == "BadPath.tsf") throw new Exception(); });

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("BadPath.tsf"), out error);
            Assert.False(result);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Returns_No_Error_If_Save_Succeeds()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("BadPath.tsf"));

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("NewFile.tsf"), out error);
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Returns_Error_If_Save_Fails()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var splitsFile = FakeSplitsFile("SomePath.tsf");
            model.LoadSplitsFile(splitsFile);

            splitsFile
                .When(n => n.Save())
                .Do(n => { if (splitsFile.FileInfo.Name == "BadPath.tsf") throw new Exception("Some error message"); });

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("BadPath.tsf"), out error);
            Assert.NotNull(error);
            Assert.NotEqual(string.Empty, error);
        }

        [Fact]
        public void SaveCurrentSplitsAs_Fires_NotifyPropertyChanged_Event_For_FileName_If_Save_Succeeds()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            model.LoadSplitsFile(FakeSplitsFile("BadPath.tsf"));

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("NewFile.tsf"), out error);
            Assert.True(eventCatcher.CaughtProperties.Contains("FileName"));
        }

        [Fact]
        public void SaveCurrentSplitsAs_Does_Not_Fire_NotifyPropertyChanged_Event_For_FileName_If_Save_Fails()
        {
            var model = new SaveLoadHandler(Substitute.For<ISplitsFacade>());
            var splitsFile = FakeSplitsFile("SomePath.tsf");
            model.LoadSplitsFile(splitsFile);

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            splitsFile
                .When(n => n.Save())
                .Do(n => { if (splitsFile.FileInfo.Name == "BadPath.tsf") throw new Exception("Some error message"); });

            string error;
            bool result = model.SaveCurrentSplitsAs(new FileInfo("BadPath.tsf"), out error);
            Assert.False(eventCatcher.CaughtProperties.Contains("FileName"));
        }
    }
}
