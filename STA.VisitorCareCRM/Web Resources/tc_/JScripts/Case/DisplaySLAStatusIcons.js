function displaySLAStatusIcons(rowData, userLCID) {
    if (rowData != null && rowData != undefined) {
        var formattedRowData = null, columnData = null, colImgName = null, colToolTip = null;
        formattedRowData = JSON.parse(rowData);

        if (formattedRowData.hasOwnProperty("firstresponseslastatus_Value")) {
            columnData = formattedRowData.firstresponseslastatus_Value;
            colToolTip = formattedRowData.firstresponseslastatus;
        }
        else if (formattedRowData.hasOwnProperty("resolvebyslastatus_Value")) {
            columnData = formattedRowData.resolvebyslastatus_Value;
            colToolTip = formattedRowData.resolvebyslastatus;
        }
    }

    switch (columnData) {
        case 1:
        case 3:
            colImgName = "tc_/Images/WithinSLA.png";
            break;
        case 2:
            colImgName = "tc_/Images/InWanring.png";
            break;        
        case 4:
            colImgName = "tc_/Images/SLABreached.png";
            break;
    }
    var resultArray = [colImgName, colToolTip];
    return resultArray;
}