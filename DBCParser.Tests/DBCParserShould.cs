using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DBCParser;
using NetCoreSystemEnvHelper;
using Xunit;
namespace DBCParser.Tests
{
    public class DBCParserShould
    {
        static string DBCFileTest = null;
        static string DBCMessageBlock = null;
        static DBCParserShould ()
        {
            if (DBCFileTest == null)
            {
                string FilePath = FileSysHelper.GetCurrentAppLocationPath () + "\\Resources\\DBCParser.TestsResources\\dbcForTest.dbc";
                DBCFileTest = File.ReadAllText (FilePath);
            }
            if (DBCMessageBlock == null)
            {
                string FilePath = FileSysHelper.GetCurrentAppLocationPath () + "\\Resources\\DBCParser.TestsResources\\dbcMessageBlockForTest.txt";
                DBCMessageBlock = File.ReadAllText (FilePath);
            }
        }

        [Fact]
        public void ParserNodesTest ()
        {
            //Given
            var res = DBCFileParser.ParserNodes (DBCFileTest);
            //When
            Assert.Equal (4, res.Where (node => node.NodeName.Equals ("EMS")).First ().TransimitMessages.Count);
            Assert.Equal (3, res.Where (node => node.NodeName.Equals ("ED")).First ().TransimitMessages.Count);
            Assert.Equal (3, res.Where (node => node.NodeName.Equals ("EMS")).First ().ReceiveMessages.Count);
            Assert.Equal (4, res.Where (node => node.NodeName.Equals ("ED")).First ().ReceiveMessages.Count);
            Assert.Equal (1, res.Where (node => node.NodeName.Equals ("BMS")).First ().ReceiveMessages.Count);
            //Then
        }

        [Fact]
        public void GetNodesTest ()
        {
            //Given
            IList<string> nodesList = DBCFileParser.GetNodes (DBCFileTest);
            //When
            Assert.Equal (3, nodesList.Count);
            //Then
        }

        [Fact]
        public void GetMessageBlockTextTest ()
        {
            IList<string> messageBlocks = DBCFileParser.GetMessageBlockText (DBCFileTest);
            // foreach (var messageBlock in messageBlocks)
            // {
            //     Console.WriteLine (messageBlock);
            // }
            Assert.Equal (7, messageBlocks.Count);
        }

        [Fact]
        public void GetSignalLineTextTest ()
        {
            //Given
            IList<string> signalLines = null;
            //When
            signalLines = DBCFileParser.GetSignalLineText (DBCFileTest);
            //Then
            Assert.Equal (41, signalLines.Count);
        }

        [Fact]
        public void ParserSignalLineTextTest1 ()
        {
            //Given
            string signalLine = " SG_ Pt_tiEdrvShOff : 50|12@1+ (1,0) [0|4095] \"min\"  ED";

            //When
            SignalItem item = DBCFileParser.ParserSignalLineText (signalLine);
            //Then
            Assert.Equal ("Pt_tiEdrvShOff", item.SignalName);
            Assert.Equal (50u, item.StartBit);
            Assert.Equal (12u, item.SignalSize);
            Assert.Equal (SignalItem.ByteOrderEnum.Intel, item.ByteOrder);
            Assert.Equal (SignalItem.ValueTypeEnum.Unsigned, item.ValueType);
            Assert.Equal (1f, item.Factor);
            Assert.Equal (0f, item.Offset);
            Assert.Equal (0u, item.Min);
            Assert.Equal (4095u, item.Max);
            Assert.Equal ("min", item.Unit);
            Assert.Contains ("ED", item.Receiver);
        }

        [Fact]
        public void ParserSignalLineTextTest2 ()
        {
            //Given
            string signalLine = " SG_ Pt_tiEdrvShOff : 50|12@1+ (1,0) [0|4095] \"min\"  ED,EEM";

            //When
            SignalItem item = DBCFileParser.ParserSignalLineText (signalLine);
            //Then
            Assert.Equal ("Pt_tiEdrvShOff", item.SignalName);
            Assert.Equal (50u, item.StartBit);
            Assert.Equal (12u, item.SignalSize);
            Assert.Equal (SignalItem.ByteOrderEnum.Intel, item.ByteOrder);
            Assert.Equal (SignalItem.ValueTypeEnum.Unsigned, item.ValueType);
            Assert.Equal (1f, item.Factor);
            Assert.Equal (0f, item.Offset);
            Assert.Equal (0u, item.Min);
            Assert.Equal (4095u, item.Max);
            Assert.Equal ("min", item.Unit);
            Assert.Equal (2, item.Receiver.Count);
            Assert.Contains ("ED", item.Receiver);
            Assert.Contains ("EEM", item.Receiver);
        }

        [Fact]
        public void MessageItemParserTextTest ()
        {
            var res = DBCFileParser.MessageItemParser (DBCMessageBlock);
            Assert.Equal ("Pt_Veh_1", res.MessageName);
            Assert.Equal (16u, res.MessageId);
            Assert.Equal (8u, res.MessageSize);
            Assert.Equal ("EMS", res.Transmitter);
            Assert.Equal (7, res.SignalList.Count);
        }

        [Fact]
        public void ParserMessageBlocksTest ()
        {
            //Given
            var res = DBCFileParser.ParserMessageBlocks (DBCFileTest);
            //When
            Assert.Equal (7, res.Count);
            //Then
        }
    }
}