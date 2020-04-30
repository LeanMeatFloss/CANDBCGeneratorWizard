using System;
using System.Collections.Generic;
using ProjectHelper;
using Xunit;
namespace ArxmlTemplateDealer.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ConfigureParametersByTemplateTest ()
        {
            string projectDirPath = @"C:\Users\ITQ2CS\rtc_sandbox\rbd_briBk10_gly_sw_Development_RWS_Hansong_Dev\rbd";
            var pathAll = ProjectFileFilter.SearchForDirectory (projectDirPath, false, new List<string> () { @"[\S]+.arxml" });
            Assert.True (pathAll.Length != 0);
        }
    }
}