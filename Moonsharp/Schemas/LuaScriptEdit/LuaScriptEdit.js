define("LuaScriptEdit", ["ModalBox"],
	function(ModalBox) {
		return {
			messages: {
			},
			attributes: {
				"Code": {
					dataValueType: Terrasoft.DataValueType.TEXT,
					isRequired: true
				}
			},
			methods: {
				/**
				 * @inheritDoc BaseSchemaViewModel#init
				 * @protected
				 * @overridden
				 */
				init: function(callback, scope) {
					this.callParent([function() {
						Terrasoft.chain(
							function() {
								if (callback) {
									callback.call(scope || this);
									ModalBox.updateSizeByContent();
								}
							},
							this
						);
					}, this]);
				},

				/**
				 * @inheritdoc Terrasoft.configuration.BaseSchemaViewModel#onRender
				 * @overridden
				 */
				onRender: function() {
					this.callParent(arguments);
					ModalBox.updateSizeByContent();
				},

				/**
				 * Closes modal window.
				 * @virtual
				 */
				onCancel: function() {
					ModalBox.close();
				},

				/**
				 * Executes Lua code.
				 * @virtual
				 */
				onExecute: function() {
					var config = this.getServiceConfig();
					this.callService(config, function(response) {
						var result = response.ExecuteResult;
						var message = result.success
							? result.Value || "Done."
							: result.errorInfo.message;
						this.showInformationDialog(message, this.Terrasoft.emptyFn);
					});
				},

				getServiceConfig: function() {
					return {
						serviceName: "LuaExecutorService",
						methodName: "Execute",
						scope: this,
						data: {
							code: this.get("Code")
						}
					};
				}
			},
			diff: [
			{
				"operation": "insert",
				"name": "WrapContainer",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.CONTAINER,
					"items": []
				}
			}, {
				"operation": "insert",
				"name": "CodeLabel",
				"parentName": "WrapContainer",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.LABEL,
					"caption": "Your code:",//{"bindTo": "Resources.Strings.SelectMailService"},
					"classes": {
						"labelClass": []
					},
					"items": []
				},
				"index": 1
			}, {
				"operation": "insert",
				"parentName": "WrapContainer",
				"propertyName": "items",
				"name": "Code",
				"values": {
					"contentType": this.Terrasoft.ContentType.LONG_TEXT,
					"labelConfig": {
						"visible": false
					},
					"value": {"bindTo": "Code"},
					"height": "500px"
				}
			}, {
				"operation": "insert",
				"name": "Cancel",
				"parentName": "WrapContainer",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					"caption": "Cancel",//{"bindTo": "Resources.Strings.Cancel"},
					"click": {
						"bindTo": "onCancel"
					},
					"style": Terrasoft.controls.ButtonEnums.style.BLUE,
					"classes": {
						"textClass": [],
						"wrapperClass": []
					},
					"items": []
				},
				"index": 2
			}, {
				"operation": "insert",
				"name": "Execute",
				"parentName": "WrapContainer",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					"caption": "Execute",//{"bindTo": "Resources.Strings.Cancel"},
					"click": {
						"bindTo": "onExecute"
					},
					"style": Terrasoft.controls.ButtonEnums.style.GREEN,
					"classes": {
						"textClass": [],
						"wrapperClass": []
					},
					"items": []
				},
				"index": 3
			}]
		};
	});
