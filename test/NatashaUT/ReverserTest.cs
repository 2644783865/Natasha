﻿using Natasha;
using Natasha.Reverser;
using NatashaUT.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NatashaUT
{
    [Trait("反解测试","参数")]
    public class ReverserTest
    {

        [Fact(DisplayName = "参数与类型反解 in")]
        public void TestIn()
        {
            var info = typeof(ReverserTestModel).GetMethod("Test1");
            Assert.Equal("in NatashaUT.Model.Rsm<NatashaUT.Model.GRsm>", DeclarationReverser.GetParametersType(info.GetParameters()[0]));
        }




        [Fact(DisplayName = "参数与类型反解 out")]
        public void TestOut()
        {
            var info = typeof(ReverserTestModel).GetMethod("Test2");
            Assert.Equal("out NatashaUT.Model.Rsm<NatashaUT.Model.Rsm<NatashaUT.Model.GRsm>[]>", DeclarationReverser.GetParametersType(info.GetParameters()[0]));
        }




        [Fact(DisplayName = "参数与类型反解 ref")]
        public void TestRef()
        {
            var info = typeof(ReverserTestModel).GetMethod("Test3");
            Assert.Equal("ref NatashaUT.Model.Rsm<NatashaUT.Model.Rsm<NatashaUT.Model.GRsm>[]>[]", DeclarationReverser.GetParametersType(info.GetParameters()[0]));
        }


        public class Inline<TT> { }

        [Fact(DisplayName = "多维数组解析")]
        public void TestType()
        {
            var temp = new { name="abc", age= 15 };
            var temp1 = new { name = "abc", age = 15, time=DateTime.Now };
            Assert.Equal("System.Nullable<System.Int32>", typeof(int?).GetDevelopName());
            Assert.Equal("<>f__AnonymousType0<System.String,System.Int32>", temp.GetType().GetDevelopName());
            Assert.Equal("<>f__AnonymousType1<System.String,System.Int32,System.DateTime>", temp1.GetType().GetDevelopName());
            Assert.Equal("System.ValueTuple<System.Int32,System.ValueTuple<System.Int32,System.Int32>>", typeof((int, (int,int))).GetDevelopName());
            Assert.Equal("System.ValueTuple<System.Int32,System.Int32>", typeof((int,int)).GetDevelopName());
            Assert.Equal("NatashaUT.ReverserTest.Inline<TT>", typeof(Inline<>).GetDevelopName());
            Assert.Equal("System.Collections.Generic.Dictionary<TKey,TValue>", typeof(Dictionary<,>).GetDevelopName());
            Assert.Equal("System.Collections.Generic.List<T>", typeof(List<>).GetDevelopName());
            Assert.Equal("System.Collections.Generic.List<System.Int32>", typeof(List<int>).GetDevelopName());
            Assert.Equal("System.Collections.Generic.List<>", typeof(List<>).GetRuntimeName());
            Assert.Equal("System.Collections.Generic.List<System.Int32>[]", typeof(List<int>[]).GetRuntimeName());
            Assert.Equal("System.Collections.Generic.List<System.Int32>[,]", typeof(List<int>[,]).GetRuntimeName());
            Assert.Equal("System.Collections.Generic.List<System.Int32>[,][][,,,,]", typeof(List<int>[,][][,,,,]).GetRuntimeName());
            Assert.Equal("System.Int32[,]", typeof(int[,]).GetRuntimeName());
            Assert.Equal("System.Int32[][]", typeof(int[][]).GetRuntimeName());
            Assert.Equal("System.Int32[][,,,]", typeof(int[][,,,]).GetRuntimeName());
            Assert.Equal("System.Collections.Generic.Dictionary<System.Int32[][,,,],System.String[,,,][]>[]", typeof(Dictionary<int[][,,,], string[,,,][]>[]).GetRuntimeName());
        }




        [Fact(DisplayName = "内部类解析")]
        public void TestInnerType()
        {

            Assert.Equal("NatashaUT.Model.OopTestModel.InnerClass", typeof(OopTestModel.InnerClass).GetRuntimeName());

        }



        [Fact(DisplayName = "类继承解析")]
        public void TestInheritanceType()
        {
            var a = typeof(InheritanceTest).GetInterfaces();
            var b = typeof(InheritanceTest).BaseType;
            Assert.Equal("NatashaUT.Model.OopTestModel.InnerClass", typeof(OopTestModel.InnerClass).GetRuntimeName());

        }


    }
}
