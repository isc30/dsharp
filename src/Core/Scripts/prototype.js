function defineInterface(namespace, interfaceName, inherits){
}

function defineAssembly(namespace, assemblyName, definition){
}

function defineClass(namespace, className, definition){
}

DSharp.defineAssembly("mscorlib", function($global, assembly){
	var ns_System = "System";
	assembly.defineInterface(ns_System, "IEquatable`1");
	assembly.defineInterface(ns_System, "IAwesome`1", function(T){ 
		return [ System.IEquatable$1(T) ] 
	});

	assembly.defineClass(ns_System, "Guid", function(staticType, instanceType){
		staticType.$interfaces = [System.IAwesome$1(System.Guid)];

		staticType.sctor = function() {
			staticType.staticValue = 0;
		}
		staticType.ctor = function(){
			staticType.ctor$1.call(this, 0);
		};
		staticType.ctor$1 = function(value){
			this.$baseType.ctor.call(this);
			this._value = value;
		};

		instanceType.toString = function(){
			return this._value;
		}
		instanceType.genericMethod = function(T, data){
			return DSharp.getDefaultTypeValue(T);
		},
		instanceType.genericMethod$1 = function(T1, T2, data){
			return DSharp.getDefaultTypeValue(T1);
		}
	});

	assembly.defineClass(ns_System, "Lazy`1", function(staticType, instanceType, T){

		staticType.ctor = function(){
			staticType.ctor$1.call(this, DSharp.getDefaultTypeValue(T1));
		};
		staticType.ctor$1 = function(value){
			this.$baseType.ctor.call(this);
			this.value = value;
			var _value = value;

			//immutable property
			DSharp.createProperty(instanceType, "ImmutableValue", this._value);
		};
		staticType.implicitLazyCastToT = function(lazy){
			return lazy.Value;
		}
		staticType.implicitTCastToLazyT = function(value){
			return new Lazy(value);
		}

		//mutable property
		DSharp.createProperty(instanceType, "Value", function(){
			return this.value;
		}, function(){
			this.value = 0;
		});
	});
});