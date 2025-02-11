﻿
// Store search filter
function StoreFilter() {
    let input = document.getElementById("textSearchStore").value.toLowerCase();

    // Filter store dropdown options
    document.querySelectorAll("#Stores option").forEach(option => {
        option.style.display = option.text.toLowerCase().includes(input) ? "" : "none";
    });

    // Filter table based on store column
    let table = document.getElementById("dataTable");
    let tr = table.getElementsByTagName("tr");

    for (let i = 1; i < tr.length; i++) {
        console.log(tr[i].id);
        let td = tr[i].querySelector("[data-column='Store']"); // Find the "Store" column
        let grandTotalRow = tr[i].id === "grandTotalRow";
        if (grandTotalRow) {
            // Ensure grand total row is always visible
            tr[i].style.display = "";

        } else if(td) {
            let textValue = td.textContent || td.innerText;
            tr[i].style.display = textValue.toLowerCase().includes(input) ? "" : "none";
        }
    }
}

// Dealer Code search filter
function DealerCodeFilter() {
    let input = document.getElementById("textSearchdealerCode").value.toLowerCase();

    // Filter store dropdown options
    document.querySelectorAll("#DealerCode option").forEach(option => {
        option.style.display = option.text.toLowerCase().includes(input) ? "" : "none";
    });

    // Filter table based on store column
    let table = document.getElementById("dataTable");
    let tr = table.getElementsByTagName("tr");

    for (let i = 1; i < tr.length; i++) {
        console.log(tr[i].id);
        let td = tr[i].querySelector("[data-column='Dealer Code']");
        let grandTotalRow = tr[i].id === "grandTotalRow";
        if (grandTotalRow) {
            // Ensure grand total row is always visible
            tr[i].style.display = "";

        } else if (td) {
            let textValue = td.textContent || td.innerText;
            tr[i].style.display = textValue.toLowerCase().includes(input) ? "" : "none";
        }
    }
}

// Market search filter
function MarketFilter() {
    let input = document.getElementById("textSearchMarket").value.toLowerCase();

    // Filter store dropdown options
    document.querySelectorAll("#Market option").forEach(option => {
        option.style.display = option.text.toLowerCase().includes(input) ? "" : "none";
    });

    // Filter table based on store column
    let table = document.getElementById("dataTable");
    let tr = table.getElementsByTagName("tr");

    for (let i = 1; i < tr.length; i++) {
        let td = tr[i].querySelector("[data-column='Market']");
        let grandTotalRow = tr[i].id === "grandTotalRow";
        if (grandTotalRow) {
            // Ensure grand total row is always visible
            tr[i].style.display = "";

        } else if (td) {
            let textValue = td.textContent || td.innerText;
            tr[i].style.display = textValue.toLowerCase().includes(input) ? "" : "none";
        }
    }
}

// TM search filter
function TMFilter() {
    let input = document.getElementById("textSearchTM").value.toLowerCase();

    // Filter store dropdown options
    document.querySelectorAll("#TM option").forEach(option => {
        option.style.display = option.text.toLowerCase().includes(input) ? "" : "none";
    });

    // Filter table based on store column
    let table = document.getElementById("dataTable");
    let tr = table.getElementsByTagName("tr");

    for (let i = 1; i < tr.length; i++) {
        let td = tr[i].querySelector("[data-column='TM']");
        let grandTotalRow = tr[i].id === "grandTotalRow";
        if (grandTotalRow) {
            // Ensure grand total row is always visible
            tr[i].style.display = "";

        } else if (td) {
            let textValue = td.textContent || td.innerText;
            tr[i].style.display = textValue.toLowerCase().includes(input) ? "" : "none";
        }
    }
}

// Role search filter
function RoleFilter() {
    let input = document.getElementById("textSearchRole").value.toLowerCase();

    // Filter store dropdown options
    document.querySelectorAll("#Role option").forEach(option => {
        option.style.display = option.text.toLowerCase().includes(input) ? "" : "none";
    });

    // Filter table based on store column
    let table = document.getElementById("dataTable");
    let tr = table.getElementsByTagName("tr");

    for (let i = 1; i < tr.length; i++) {
        let td = tr[i].querySelector("[data-column='Role']");
        let grandTotalRow = tr[i].id === "grandTotalRow";
        if (grandTotalRow) {
            // Ensure grand total row is always visible
            tr[i].style.display = "";
        } else if (td) {
            let textValue = td.textContent || td.innerText;
            tr[i].style.display = textValue.toLowerCase().includes(input) ? "" : "none";
        }
    }
}


// Table filter
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

//function dateFilterTable() {
function dateFilterTable() {
    // Get the selected date in YYYY-MM-DD format
    var selectedDate = document.getElementById('selectedDate').value;

    // Convert selectedDate to YYYYMMDD format
    if (selectedDate !== "") {
        selectedDate = selectedDate.replace(/-/g, ""); // Convert to YYYYMMDD
    }

    var table = document.getElementById('dataTable');
    var rows = table.getElementsByTagName('tr'); // Get all rows of the table
    var headers = table.getElementsByTagName('th'); // Get the headers
    var dateColumnIndex = -1;

    // Find the index of the DateKey column by searching headers
    for (var i = 0; i < headers.length; i++) {
        if (headers[i].textContent.trim().toLowerCase().includes("date") || headers[i].textContent.trim().toLowerCase().includes("datekey")) {
            dateColumnIndex = i;
            break;
        }
    }

    // If the DateKey column is not found, exit the function
    if (dateColumnIndex === -1) {
        console.error("Date column not found.");
        return;
    }

    // Loop through all rows (skip the header and grand total row)
    for (var i = 1; i < rows.length; i++) {
        // Get all the table cells in the row
        var cells = rows[i].getElementsByTagName('td');

        // Check if the row has the expected number of cells (to avoid processing headers)
        if (cells.length > 0) {
            // Get the DateKey value from the detected column
            var rowDateKey = cells[dateColumnIndex].textContent.trim(); // Use the dynamically found column

            // If selectedDate is empty, show all rows
            if (selectedDate === "" || rowDateKey === selectedDate) {
                rows[i].style.display = ""; // Show row
            } else {
                rows[i].style.display = "none"; // Hide row
            }
        }
    }
}

function refresh() {
    window.location.reload();
}






