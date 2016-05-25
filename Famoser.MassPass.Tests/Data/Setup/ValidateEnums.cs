using System;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Data.Attributes;
using Famoser.MassPass.Data.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Setup
{
    [TestClass]
    public class ValidateEnums
    {
        [TestMethod]
        public void AllApiErrorValuesHaveDescriptionAttribute()
        {
            AllEnumValuesHaveAttribute<ApiError, DescriptionAttribute>();
        }

        [TestMethod]
        public void AllApiRequestValuesHaveApiUrlAttribute()
        {
            AllEnumValuesHaveAttribute<ApiRequest, ApiUriAttribute>();
        }

        [TestMethod]
        public void AllContentStatusValuesHaveDescriptionAttribute()
        {
            AllEnumValuesHaveAttribute<ContentStatus, DescriptionAttribute>();
        }

        [TestMethod]
        public void AllLocalStatusValuesHaveDescriptionAttribute()
        {
            AllEnumValuesHaveAttribute<LocalStatus, DescriptionAttribute>();
        }

        [TestMethod]
        public void AllRemoteStatusValuesHaveDescriptionAttribute()
        {
            AllEnumValuesHaveAttribute<RemoteStatus, DescriptionAttribute>();
        }

        private void AllEnumValuesHaveAttribute<TE, TA>() where TA : Attribute
        {
            //arrange
            var vals = Enum.GetValues(typeof(TE));

            //act & assert
            foreach (var val in vals)
            {
                ReflectionHelper.GetAttributeOfEnum<TA, TE>((TE)val);
            }
        }
    }
}
