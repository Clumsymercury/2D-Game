extends Control
#@onready var inv: Inv = preload("res://inventory/player_inv.tres")
var inv: Inv = null
@onready var slots: Array = $NinePatchRect/GridContainer.get_children()

var is_open = false

func _ready():
	await get_tree().process_frame  # ✅ ensures all nodes (like GDscript) are ready
	var inventory_node = get_parent().get_node_or_null("GDscript")
	if inventory_node == null:
		push_error("Inventory node not found!")
		return

	inv = inventory_node.inv
	if inv == null:
		push_error("Inventory data not assigned!")
		return
	inv.update.connect(update_slots)
	update_slots()
	close()
func update_slots():
	for i in range(min(inv.slots.size(), slots.size())):
		slots[i].update(inv.slots[i])
func _process(delta):
	if Input.is_action_just_pressed("inventory"):
		if is_open:
			close ()
		else:
			open()

func open():
	self. visible = true
	is_open = true 
	
func close():	
	visible = false;
	is_open = false;
	
