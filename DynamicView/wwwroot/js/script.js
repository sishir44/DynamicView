// Table Data filter
function TableDataFilter() {
    var input = document.getElementById("textSearchTable").value.toUpperCase();
    var table = document.getElementById("dataTable");
    var tr = table.getElementsByTagName("tr");
    var selectedColumn = document.getElementById("filterColumn").value;

    for (var i = 1; i < tr.length; i++) {
        var tds = tr[i].getElementsByTagName("td");
        var match = false;

        for (var j = 0; j < tds.length; j++) {
            if (tds[j]) {
                var textValue = tds[j].textContent || tds[j].innerText;
                var columnName = tds[j].getAttribute("data-column");

                if (textValue.toUpperCase().indexOf(input) > -1 && (selectedColumn === "" || columnName === selectedColumn)) {
                    match = true;
                    break;
                }
            }
        }
        tr[i].style.display = match ? "" : "none";  // Show or hide the row based on the match
    }
}

// refesh button
window.refresh = function () {
    window.location.reload();
};
