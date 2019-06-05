﻿using System;
using System.Collections.Concurrent;
using System.Text;

namespace Natasha
{
    public class CloneBuilder<T>
    {
        public static void CreateCloneDelegate()
        {
            CloneOperator<T>.CloneDelegate =(Func<T, T>)((new CloneBuilder(typeof(T)).Create())); ;
        }
    }
    public class CloneBuilder : TypeIterator
    {
        public readonly StringBuilder Script;
        private readonly FastMethodOperator MethodHandler;
        private const string NewInstance = "NewInstance";
        private const string OldInstance = "OldInstance";
        public static readonly ConcurrentDictionary<Type, Delegate> CloneCache;

        static CloneBuilder() => CloneCache = new ConcurrentDictionary<Type, Delegate>();

        public CloneBuilder(Type type=null) {
            CurrentType = type;
            Script = new StringBuilder();
            MethodHandler = new FastMethodOperator();
        }



        public override void EntityHandler(Type type)
        {
            MethodHandler.Using("Natasha");
            CloneBuilder builder = new CloneBuilder(type);
            builder.Create();
        }




        public override void ArrayOnceTypeHandler(BuilderInfo info)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"if(oldInstance!=null){");
            scriptBuilder.Append($"var newInstance = new {info.TypeName}[oldInstance.Length];");
            //普通类型复制
            scriptBuilder.Append(
                $@"for (int i = 0; i < oldInstance.Length; i++){{
                    newInstance[i] = oldInstance[i];
                 }}return newInstance;}}return null;");

            //创建委托
            var tempBuilder = FastMethodOperator.New;
            tempBuilder.ComplierInstance.UseFileComplie();
            tempBuilder.Using(info.RealType);
            CloneCache[info.Type] = tempBuilder
                        .Using("Natasha")
                        .ClassName("NatashaClone" + info.AvailableName)
                        .MethodName("Clone")
                        .Param(info.Type, "oldInstance")                 //参数
                        .MethodBody(scriptBuilder.ToString())            //方法体
                        .Return(info.Type)                               //返回类型
                        .Complie();
        }





        public override void ArrayEntityHandler(BuilderInfo info)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"if(oldInstance!=null){");
            scriptBuilder.Append($"var newInstance = new {info.TypeName}[oldInstance.Length];");
            //普通类型复制
            scriptBuilder.Append(
                $@"for (int i = 0; i < oldInstance.Length; i++){{
                    newInstance[i] =  NatashaClone{AvailableNameReverser.GetName(info.RealType)}.Clone(oldInstance[i]);
                 }}return newInstance;}}return null;");

            //创建委托
            var tempBuilder = FastMethodOperator.New;
            tempBuilder.ComplierInstance.UseFileComplie();
            tempBuilder.Using(info.RealType);
            CloneCache[info.Type] = tempBuilder
                        .Using("Natasha")
                        .ClassName("NatashaClone" + info.AvailableName)
                        .MethodName("Clone")
                        .Param(info.Type, "oldInstance")                 //参数
                        .MethodBody(scriptBuilder.ToString())            //方法体
                        .Return(info.Type)                               //返回类型
                        .Complie();
        }




        public override void CollectionHandler(BuilderInfo info)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"if(oldInstance!=null){");
            scriptBuilder.Append($"return new {info.TypeName}(oldInstance.CloneExtension());");
            scriptBuilder.Append("}return null;");


            //创建委托
            var tempBuilder = FastMethodOperator.New;
            tempBuilder.Using(info.RealType.GetGenericArguments());
            tempBuilder.Using("Natasha");
            tempBuilder.ComplierInstance.UseFileComplie();
            CloneCache[info.RealType] = tempBuilder
                        .Using("Natasha")
                        .Using(info.RealType)
                        .Using(GenericTypeOperator.GetTypes(info.RealType))
                        .ClassName("NatashaClone" +info.AvailableName)
                        .MethodName("Clone")
                        .Param(info.RealType, "oldInstance")                 //参数
                        .MethodBody(scriptBuilder.ToString())                //方法体
                        .Return(info.RealType)                               //返回类型
                        .Complie();
        }




        public override void ICollectionHandler(BuilderInfo info)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"if(oldInstance!=null){");
            scriptBuilder.Append($"return oldInstance.CloneExtension();");
            scriptBuilder.Append("}return null;");

            //创建委托
            var tempBuilder = FastMethodOperator.New;
            tempBuilder.Using(info.RealType.GetGenericArguments());
            tempBuilder.Using("Natasha");
            tempBuilder.ComplierInstance.UseFileComplie();
            CloneCache[info.RealType] = tempBuilder
                        .Using("Natasha")
                        .Using(info.RealType)
                        .Using(GenericTypeOperator.GetTypes(info.RealType))
                        .ClassName("NatashaClone" + info.AvailableName)
                        .MethodName("Clone")
                        .Param(info.RealType, "oldInstance")                 //参数
                        .MethodBody(scriptBuilder.ToString())                //方法体
                        .Return(info.RealType)                               //返回类型
                        .Complie();
        }




        public override void EntityStartHandler(BuilderInfo info)
        {
            Script.Append($"if({OldInstance}==null){{return null;}}");
            Script.Append($"{info.TypeName} {NewInstance} = new {info.TypeName}();");
        }




        public override void EntityReturnHandler(BuilderInfo info)
        {
            Script.Append($"return {NewInstance};");
        }




        public override void MemberOnceTypeHandler(BuilderInfo info)
        {
            Script.Append($"{NewInstance}.{info.MemberName} = {OldInstance}.{info.MemberName};");
        }





        public override void MemberArrayOnceTypeHandler(BuilderInfo info)
        {
            MethodHandler.Using(info.Type);
            Script.Append($@"if({OldInstance}.{info.MemberName}!=null){{");
            Script.Append($"{NewInstance}.{info.MemberName} = new {info.TypeName}[{OldInstance}.{info.MemberName}.Length];");
            //普通类型复制
            Script.Append(
                $@"for (int i = 0; i < {OldInstance}.{info.MemberName}.Length; i++){{
                      {NewInstance}.{info.MemberName}[i] ={OldInstance}.{info.MemberName}[i];
                }}}}");
        }




        public override void MemberArrayEntityHandler(BuilderInfo info)
        {
            MethodHandler.Using(info.RealType);
            Script.Append($@"if({OldInstance}.{info.MemberName}!=null){{");
            Script.Append($"{NewInstance}.{info.MemberName} = new {info.TypeName}[{OldInstance}.{info.MemberName}.Length];");
            //普通类型复制
            Script.Append(
                $@"for (int i = 0; i < {OldInstance}.{info.MemberName}.Length; i++){{
                      {NewInstance}.{info.MemberName}[i] = NatashaClone{AvailableNameReverser.GetName(info.RealType)}.Clone({OldInstance}.{info.MemberName}[i]);
                }}}}");
        }




        public override void MemberICollectionHandler(BuilderInfo info)
        {
            MethodHandler.Using(info.RealType);
            Script.Append($@"if({OldInstance}.{info.MemberName}!=null){{");
            Script.Append($@"{NewInstance}.{info.MemberName} = {OldInstance}.{info.MemberName}.CloneExtension();}}");
        }




        public override void MemberCollectionHandler(BuilderInfo info)
        {
            MethodHandler.Using(info.RealType);
            Script.Append($@"if({OldInstance}.{info.MemberName}!=null){{");
            Script.Append($@"{NewInstance}.{info.MemberName} = new  {info.TypeName}({OldInstance}.{info.MemberName}.CloneExtension());}}");
        }




        public override void MemberEntityHandler(BuilderInfo info)
        {
            MethodHandler.Using("Natasha");
            Script.Append($"if({OldInstance}.{info.MemberName}!=null){{");
            Script.Append($"{NewInstance}.{info.MemberName} = NatashaClone{info.AvailableName}.Clone({OldInstance}.{info.MemberName});}}");
        }




        public Delegate Create()
        {
            TypeHandler(CurrentType);
            //创建委托
            MethodHandler.ComplierInstance.UseFileComplie();
            var @delegate = MethodHandler
                        .ClassName("NatashaClone" + AvailableNameReverser.GetName(CurrentType))
                        .MethodName("Clone")
                        .Param(CurrentType, OldInstance)                //参数
                        .MethodBody(Script.ToString())                 //方法体
                        .Return(CurrentType)                              //返回类型
                       .Complie();
            return CloneCache[CurrentType] = @delegate;
        }
    }
}
