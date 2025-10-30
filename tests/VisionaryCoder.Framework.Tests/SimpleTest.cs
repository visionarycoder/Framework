// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisionaryCoder.Framework.Tests;

[TestClass]
public class SimpleTest
{
    [TestMethod]
    public void SimpleTest_ShouldPass()
    {
        // Arrange
        var expected = true;
        
        // Act
        var actual = true;
        
        // Assert
        Assert.AreEqual(expected, actual);
    }
}