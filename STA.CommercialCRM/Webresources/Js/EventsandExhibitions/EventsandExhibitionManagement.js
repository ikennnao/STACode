function checkEventStartDate(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var StartDate = GlobalformContext.getAttribute('cc_startdate').getValue();
    var currentDate = new Date();
    if (StartDate != null) {
        StartDate.setHours(0, 0, 0, 0);
        // rest of the code
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        if (StartDate < today) {
            Xrm.Utility.alertDialog("Start Date can not be Less than Today");
            GlobalformContext.getAttribute('cc_startdate').setValue(null);
        }
    }
}

function checkEventEndDate(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var EndDate = GlobalformContext.getAttribute('cc_enddate').getValue();
    var currentDate = new Date();
    if (EndDate != null) {
        EndDate.setHours(0, 0, 0, 0);
        // rest of the code
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        if (EndDate < today) {
            Xrm.Utility.alertDialog("End Date can not be Less than Today");
            GlobalformContext.getAttribute('cc_enddate').setValue(null);
        }
    }
}