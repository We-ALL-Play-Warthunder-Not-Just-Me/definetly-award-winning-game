@tool
extends EditorPlugin

var dock
func _enable_plugin() -> void:
	# Add autoloads here.
	pass

func _disable_plugin() -> void:
	# Remove autoloads here.
	pass

func _enter_tree() -> void:
	# Initialization of the plugin goes here.
	var dock_scene = preload("uid://cj3cvffgjdnae").instantiate()
	dock = EditorDock.new()
	dock.add_child(dock_scene);
	
	dock.title = "Teleport";
	dock.default_slot = EditorDock.DOCK_SLOT_RIGHT_BR
	dock.available_layouts = EditorDock.DOCK_LAYOUT_VERTICAL | EditorDock.DOCK_LAYOUT_FLOATING
	
	add_dock(dock)
	pass


func _exit_tree() -> void:
	# Clean-up of the plugin goes here.
	remove_dock(dock)
	dock.queue_free()
	pass
