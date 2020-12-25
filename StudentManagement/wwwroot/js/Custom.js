function confirmDelete(uniqueId,isDeleteCliked)
{
	var deletespan = "deleteSpan_" + uniqueId;
	var confirmDeleteSpan = "confirmdelete_" + uniqueId;

	if (isDeleteCliked) {
		$("#" + deletespan).hide();
		$("#" + confirmDeleteSpan).show();

	} else {
		$("#" + deletespan).show();
		$("#" + confirmDeleteSpan).hide();
	}
}