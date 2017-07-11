define("LuaSourceCodeEditPage", ["terrasoft", "LuaSourceCodeEdit"], function(Terrasoft) {
	define("LuaSourceCodeEditGenerator", ["SourceCodeEditGenerator"], function(){
		return Ext.create("Terrasoft.SourceCodeEditGenerator", {
			sourceCodeEditClassName: "Terrasoft.LuaSourceCodeEdit"
		})
	});
	return {
		diff: [{
			"operation": "merge",
			"name": "SourceCode",
			"values": {
				"generator": "LuaSourceCodeEditGenerator.generate"
			}
		}]
	};
	
});