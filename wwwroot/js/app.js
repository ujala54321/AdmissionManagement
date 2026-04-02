// ─────────────────────────────────────────────────────────────
// ADMISSION MANAGEMENT SYSTEM - APP.JS
// ─────────────────────────────────────────────────────────────

$(document).ready(function () {
    initializeApp();
});

// Initialize App
function initializeApp() {
    setupEventListeners();
    setupValidation();
    loadDashboardData();
}

// ─────────────────────────────────────────────────────────────
// EVENT LISTENERS
// ─────────────────────────────────────────────────────────────

function setupEventListeners() {
    // Form submissions
    $(document).on('submit', 'form', function (e) {
        if (!validateForm($(this))) {
            e.preventDefault();
        }
    });

    // Delete confirmations
    $(document).on('click', '.btn-delete', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        if (confirm('Are you sure you want to delete this item?')) {
            deleteItem(id, $(this).data('url'));
        }
    });

    // Edit buttons
    $(document).on('click', '.btn-edit', function () {
        const id = $(this).data('id');
        loadEditForm(id, $(this).data('url'));
    });

    // Search functionality
    $(document).on('click', '#btnSearch', function () {
        const searchQuery = $('#searchName').val();
        const filterStatus = $('#filterStatus').val();
        performSearch(searchQuery, filterStatus);
    });

    // Modal handling
    $(document).on('shown.bs.modal', '.modal', function () {
        $(this).find('input:first').focus();
    });

    // Alert auto-dismiss
    $('.alert').each(function () {
        setTimeout(() => {
            $(this).fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    });
}

// ─────────────────────────────────────────────────────────────
// FORM VALIDATION
// ─────────────────────────────────────────────────────────────

function setupValidation() {
    // Bootstrap form validation
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
}

function validateForm(form) {
    let isValid = true;
    const fields = form.find('input, select, textarea');

    fields.each(function () {
        const field = $(this);
        const value = field.val().trim();
        const required = field.prop('required');
        const type = field.attr('type');

        // Clear previous error
        field.removeClass('is-invalid');
        field.siblings('.invalid-feedback').remove();

        // Validate required
        if (required && !value) {
            field.addClass('is-invalid');
            field.after(`<div class="invalid-feedback">This field is required</div>`);
            isValid = false;
            return;
        }

        // Validate email
        if (type === 'email' && value && !isValidEmail(value)) {
            field.addClass('is-invalid');
            field.after(`<div class="invalid-feedback">Please enter a valid email</div>`);
            isValid = false;
        }

        // Validate phone
        if (type === 'tel' && value && !isValidPhone(value)) {
            field.addClass('is-invalid');
            field.after(`<div class="invalid-feedback">Please enter a valid phone number</div>`);
            isValid = false;
        }

        // Validate number
        if (type === 'number' && value && isNaN(value)) {
            field.addClass('is-invalid');
            field.after(`<div class="invalid-feedback">Please enter a valid number</div>`);
            isValid = false;
        }
    });

    return isValid;
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidPhone(phone) {
    const phoneRegex = /^[\d\s\-\+\(\)]{10,}$/;
    return phoneRegex.test(phone);
}

// ─────────────────────────────────────────────────────────────
// API CALLS
// ─────────────────────────────────────────────────────────────

function deleteItem(id, url) {
    $.ajax({
        url: url || `/api/delete/${id}`,
        type: 'DELETE',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            showNotification('Item deleted successfully', 'success');
            setTimeout(() => location.reload(), 1500);
        },
        error: function (xhr) {
            showNotification('Error deleting item', 'error');
            console.error(xhr);
        }
    });
}

function loadEditForm(id, url) {
    $.ajax({
        url: url || `/api/get/${id}`,
        type: 'GET',
        success: function (response) {
            // Populate form with response data
            console.log('Edit data loaded:', response);
            showNotification('Form loaded', 'info');
        },
        error: function (xhr) {
            showNotification('Error loading form', 'error');
        }
    });
}

function performSearch(query, status) {
    if (!query && !status) {
        showNotification('Please enter search criteria', 'warning');
        return;
    }

    const url = new URL(window.location);
    url.searchParams.set('search', query);
    url.searchParams.set('status', status);
    window.location.href = url.toString();
}

// ─────────────────────────────────────────────────────────────
// NOTIFICATIONS
// ─────────────────────────────────────────────────────────────

function showNotification(message, type = 'info') {
    const alertClass = `alert-${type}`;
    const icon = getIconByType(type);
    const alertHTML = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    $('.container-fluid').prepend(alertHTML);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        $('.alert').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

function getIconByType(type) {
    const icons = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-circle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle',
        'danger': 'fa-times-circle'
    };
    return icons[type] || 'fa-info-circle';
}

// ─────────────────────────────────────────────────────────────
// DASHBOARD FUNCTIONS
// ─────────────────────────────────────────────────────────────

function loadDashboardData() {
    if (!$('.dashboard-container').length) return;

    $.ajax({
        url: '/api/dashboard/data',
        type: 'GET',
        success: function (data) {
            updateDashboard(data);
        },
        error: function (xhr) {
            console.error('Error loading dashboard data:', xhr);
        }
    });
}

function updateDashboard(data) {
    // Update metric cards
    if (data.totalIntake) {
        $('.metric-value').eq(0).text(data.totalIntake);
    }
    if (data.totalAllocated) {
        $('.metric-value').eq(1).text(data.totalAllocated);
    }
    // ... Update other metrics
}

// ─────────────────────────────────────────────────────────────
// TABLE FUNCTIONS
// ─────────────────────────────────────────────────────────────

function initializeDataTable(selector) {
    $(selector).DataTable({
        'paging': true,
        'pageLength': 10,
        'searching': true,
        'ordering': true,
        'info': true,
        'responsive': true,
        'language': {
            'lengthMenu': 'Show _MENU_ entries',
            'search': 'Search:',
            'emptyTable': 'No data available',
            'zeroRecords': 'No matching records found'
        }
    });
}

// ─────────────────────────────────────────────────────────────
// UTILITY FUNCTIONS
// ─────────────────────────────────────────────────────────────

function formatDate(date) {
    return new Date(date).toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'short',
        day: '2-digit'
    });
}

function formatCurrency(value) {
    return new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'INR'
    }).format(value);
}

function throttle(func, limit) {
    let inThrottle;
    return function () {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

function debounce(func, delay) {
    let timeoutId;
    return function (...args) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => func.apply(this, args), delay);
    };
}

// ─────────────────────────────────────────────────────────────
// EXPORT FUNCTIONS
// ─────────────────────────────────────────────────────────────

function exportToCSV(filename = 'export.csv') {
    const csv = [];
    const rows = document.querySelectorAll('table tr');

    rows.forEach(row => {
        const cols = row.querySelectorAll('td, th');
        const csvRow = [];
        cols.forEach(col => {
            csvRow.push(col.innerText);
        });
        csv.push(csvRow.join(','));
    });

    downloadFile(csv.join('\n'), filename, 'text/csv');
}

function exportToExcel(filename = 'export.xlsx') {
    const table = document.querySelector('table');
    const workbook = XLSX.utils.table_to_book(table);
    XLSX.writeFile(workbook, filename);
}

function downloadFile(content, filename, type) {
    const blob = new Blob([content], { type: type });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
}

// ─────────────────────────────────────────────────────────────
// PRINT FUNCTIONS
// ─────────────────────────────────────────────────────────────

function printDocument(elementId = null) {
    const printWindow = window.open('', '', 'width=900,height=600');
    const printContent = elementId ? document.getElementById(elementId).innerHTML : document.body.innerHTML;

    printWindow.document.write(`
        <html>
        <head>
            <title>Print Document</title>
            <link rel="stylesheet" href="/css/bootstrap.min.css">
            <link rel="stylesheet" href="/css/style.css">
            <style>
                body { margin: 20px; }
                @media print {
                    .no-print { display: none; }
                }
            </style>
        </head>
        <body>
            ${printContent}
            <script>
                window.addEventListener('load', function() {
                    window.print();
                    window.close();
                });
            </script>
        </body>
        </html>
    `);
    printWindow.document.close();
}

// ─────────────────────────────────────────────────────────────
// LOGGING
// ─────────────────────────────────────────────────────────────

const Logger = {
    log: function (message, type = 'info') {
        console.log(`[${type.toUpperCase()}] ${message}`);
    },
    error: function (message) {
        console.error(`[ERROR] ${message}`);
    },
    warn: function (message) {
        console.warn(`[WARN] ${message}`);
    },
    debug: function (message) {
        if (window.DEBUG_MODE) {
            console.debug(`[DEBUG] ${message}`);
        }
    }
};

// ─────────────────────────────────────────────────────────────
// ERROR HANDLING
// ─────────────────────────────────────────────────────────────

window.addEventListener('error', function (event) {
    Logger.error(`Uncaught Error: ${event.message}`);
    showNotification('An unexpected error occurred', 'error');
});

window.addEventListener('unhandledrejection', function (event) {
    Logger.error(`Unhandled Promise Rejection: ${event.reason}`);
    showNotification('An unexpected error occurred', 'error');
});