//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using nanoFramework.Networking.Thread;

namespace DataSetTests
{
    [TestClass]
    public class DataSetTests
    {
        byte[] NETKEY = new byte[32] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2 };
        ushort PANID = 0x4321;
        ushort CHANNEL = 13;
        byte[] EXPANID = new byte[8] { 0, 1, 2, 3, 4, 3, 2, 1 };
        byte[] PSKC = new byte[16] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

        [TestMethod]
        public void TestConstructors()
        {
            // Create dataset
            OpenThreadDataset ds = new OpenThreadDataset()
            {
                NetworkKey = NETKEY,
                PanId = PANID,
                Channel = CHANNEL,
                ExtendedPanId = EXPANID,
                PSKc = PSKC
            };

            // Check properties set
            Assert.IsTrue(ds.NetworkKey.SequenceEqual(NETKEY), "Network key wrong.");
            Assert.IsTrue(ds.PanId == PANID, "Panid is incorrect.");
            Assert.IsTrue(ds.Channel == CHANNEL, "Channel is incorrect.");
            Assert.IsTrue(ds.ExtendedPanId.SequenceEqual(EXPANID), "Expanid is incorrect");
            Assert.IsTrue(ds.PSKc.SequenceEqual(PSKC), "PSKc is incorrect");
        }
    }
}
