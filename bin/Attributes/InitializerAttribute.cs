using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TOCS.Modules;

namespace TOCS.Attributes
{

[AttributeUsage(AttributeTargets.Method)]
public abstract class InitializerAttribute<T> : Attribute
{
        /// <summary>全初期化メソッド</summary>
        private static MethodInfo[] allInitializers = null;
        private static LogHandler logger = Logger.Handler(nameof(InitializerAttribute<T>));

        public InitializerAttribute() : this(InitializePriority.Normal) { }
        public InitializerAttribute(InitializePriority priority)
        {
            this.priority = priority;
        }

        private readonly InitializePriority priority = InitializePriority.Normal;
        /// <summary>初期化時に呼び出されるメソッド</summary>
        private MethodInfo targetMethod;

        private static void FindInitializers()
        {
            var initializers = new HashSet<InitializerAttribute<T>>(32);

            // TownOfHost.dll内の
            var assembly = Assembly.GetExecutingAssembly();
            // 全クラス内の
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // 全メソッドについて
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    // InitializerAttributeを取得
                    var attribute = method.GetCustomAttribute<InitializerAttribute<T>>();
                    if (attribute != null)
                    {
                        // 取得できたら登録
                        attribute.targetMethod = method;
                        initializers.Add(attribute);
                    }
                }
            }
            // 見つかった初期化メソッドをpriority順に並べ替えて配列に変換
            allInitializers = initializers.OrderBy(initializer => initializer.priority).Select(initializer => initializer.targetMethod).ToArray();
        }
        public static void InitializeAll()
        {
            // 初回の初期化時に初期化メソッドを探す
            if (allInitializers == null)
            {
                FindInitializers();
            }
            foreach (var initializer in allInitializers)
            {
                logger.Info($"初始化: {initializer.DeclaringType.Name}.{initializer.Name}");
                initializer.Invoke(null, null);
            }
        }
    }

    public enum InitializePriority
    {
        /// <summary>既定値</summary>
        Normal,
    }
}