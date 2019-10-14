using System;
using System.Linq;
using System.Collections.Generic;

namespace CustomIoC.Core
{
	public class Container
	{
		private readonly Dictionary<Type, Type> map = new Dictionary<Type, Type>();

		public ContainerBuilder For<TSource>()
		{
			return For(typeof(TSource));
		}

		public ContainerBuilder For(Type sourceType)
		{
			return new ContainerBuilder(this, sourceType);
		}

		public TSource Resolve<TSource>()
		{
			return (TSource)Resolve(typeof(TSource));
		}

		private object Resolve(Type sourceType)
		{
			if (this.map.ContainsKey(sourceType))
			{
				var destinationType = this.map[sourceType];
				object destinationInstance = this.CreateInstance(destinationType);
				return destinationInstance;
			}
			else if (!sourceType.IsAbstract)
			{
				return this.CreateInstance(sourceType);
			}
			else if (sourceType.IsGenericType && this.map.ContainsKey(sourceType.GetGenericTypeDefinition()))
			{
				var destinationType = this.map[sourceType.GetGenericTypeDefinition()];
				var closedDestinationType = destinationType.MakeGenericType(sourceType.GetGenericArguments());
				var closedDestinationInstance = this.CreateInstance(closedDestinationType);
				return closedDestinationInstance;
			}
			else
			{
				throw new InvalidOperationException($"Could not resolve {sourceType.FullName}");
			}
		}

		private object CreateInstance(Type destinationType)
		{
			var parametrs = destinationType.GetConstructors()
				.OrderByDescending(c => c.GetParameters().Count())
				.First()
				.GetParameters()
				.Select(p => this.Resolve(p.ParameterType))
				.ToArray();

			return Activator.CreateInstance(destinationType, parametrs);
		}

		public class ContainerBuilder
		{
			private Container container;
			private Type sourceType;

			public ContainerBuilder(Container container, Type sourceType)
			{
				this.container = container;
				this.sourceType = sourceType;
			}

			public ContainerBuilder Use<TDestination>()
			{
				return Use(typeof(TDestination));
			}

			public ContainerBuilder Use(Type destinationType)
			{
				this.container.map.Add(this.sourceType, destinationType);
				return this;
			}
		}
	}
}
