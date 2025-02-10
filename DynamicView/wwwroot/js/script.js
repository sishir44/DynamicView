
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
        let td = tr[i].querySelector("[data-column='Store']"); // Find the "Store" column
        if (td) {
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
        let td = tr[i].querySelector("[data-column='Dealer Code']");
        if (td) {
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
        if (td) {
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
        if (td) {
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
        if (td) {
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




