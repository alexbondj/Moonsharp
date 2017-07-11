define("LuaSourceCodeEdit", ["ext-base", "SourceCodeEdit", "AceLua"],
	function(Ext) {
		return Ext.define("Terrasoft.configuration.LuaSourceCodeEdit", {
			extend: "Terrasoft.SourceCodeEdit",
			alternateClassName: "Terrasoft.LuaSourceCodeEdit",
			workerModeInitialized: false,

			getEditorMode: function(language) {
				if (!this.workerModeInitialized && this.aceEdit){
					this.workerModeInitialized = true;
					var aceEdit = this.aceEdit;
					aceEdit.session.setOption("useWorker", false);
				}
				if (language === "lua"){
					return "ace/mode/lua"
				}
				return this.callParent(arguments);
			}

		});
	}
);
