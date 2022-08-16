//javascript
function createCase(primaryControl)
{
  var entityFormOptions = {}, formParameters = {};
    entityFormOptions["entityName"] = "incident";
    entityFormOptions["useQuickCreateForm"] = true;
    entityFormOptions["windowPosition"] = 1;
    
    
     // Open the Quick Create form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                SetAttributeValue(executionContext, "tc_requesttype", 2);
                RefreshFormRibbon(executionContext, false);
            }
        },
        function (error) {
            alert(error);
        });
}