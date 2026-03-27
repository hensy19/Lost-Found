/* ============================================
   SHARED VALIDATION HELPERS
   ============================================ */

function isValidEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

function addError(fieldId, errorId, message) {
    const field = document.getElementById(fieldId);
    const errorDisplay = document.getElementById(errorId);
    if (field) field.classList.add('is-invalid');
    if (errorDisplay) errorDisplay.textContent = message;
}

function clearAllErrors() {
    const container = document.getElementById('validationErrors');
    const list = document.getElementById('errorList');
    if (container) container.style.display = 'none';
    if (list) list.innerHTML = '';
    
    document.querySelectorAll('.form-control.is-invalid').forEach(field => {
        field.classList.remove('is-invalid');
    });
    document.querySelectorAll('.input-error-text').forEach(error => {
        error.textContent = '';
    });
}

function displayErrors(errorArray) {
    const errorContainer = document.getElementById('validationErrors');
    const errorList = document.getElementById('errorList');
    if (!errorContainer || !errorList) return;

    errorList.innerHTML = '';
    errorArray.forEach(error => {
        const li = document.createElement('li');
        li.textContent = error;
        errorList.appendChild(li);
    });
    errorContainer.style.display = 'block';
}

// Global Event Listeners for Resetting Errors
document.addEventListener('input', function(e) {
    if (e.target.classList.contains('form-control')) {
        e.target.classList.remove('is-invalid');
        // Find corresponding error span and clear it
        const errorSpan = document.getElementById(e.target.id + 'Error');
        if (errorSpan) errorSpan.textContent = '';
    }
});

document.addEventListener('change', function(e) {
    if (e.target.id === 'terms') {
        const termsErr = document.getElementById('termsError');
        if (termsErr) termsErr.textContent = '';
    }
});

/* ============================================
   FORM-SPECIFIC VALIDATORS
   ============================================ */

function validateForm(event) {
    event.preventDefault();
    const form = event.target;
    clearAllErrors();
    
    const fullName = document.getElementById('fullName').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const termsEl = document.getElementById('terms');
    const terms = termsEl ? termsEl.checked : true;
    
    const errors = [];
    let isValid = true;

    if (!fullName) {
        addError('fullName', 'fullNameError', 'Full name is required');
        errors.push('Full name is required');
        isValid = false;
    } else if (fullName.length < 3) {
        addError('fullName', 'fullNameError', 'Full name must be at least 3 characters');
        errors.push('Full name must be at least 3 characters');
        isValid = false;
    }

    if (!email) {
        addError('email', 'emailError', 'Email address is required');
        errors.push('Email address is required');
        isValid = false;
    } else if (!isValidEmail(email)) {
        addError('email', 'emailError', 'Invalid email format');
        errors.push('Invalid email format');
        isValid = false;
    }

    if (!password) {
        addError('password', 'passwordError', 'Password is required');
        errors.push('Password is required');
        isValid = false;
    } else if (password.length < 6) {
        addError('password', 'passwordError', 'Password must be at least 6 characters');
        errors.push('Password must be at least 6 characters');
        isValid = false;
    }

    if (!confirmPassword) {
        addError('confirmPassword', 'confirmPasswordError', 'Confirm password is required');
        errors.push('Confirm password is required');
        isValid = false;
    } else if (password !== confirmPassword) {
        addError('confirmPassword', 'confirmPasswordError', 'Passwords do not match');
        errors.push('Passwords do not match');
        isValid = false;
    }

    if (!terms) {
        const termsErr = document.getElementById('termsError');
        if (termsErr) termsErr.textContent = 'You must agree to the Community Guidelines';
        errors.push('You must agree to the Community Guidelines');
        isValid = false;
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    form.submit();
}

function validateLoginForm(event) {
    event.preventDefault();
    const form = event.target;
    clearAllErrors();
    
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    
    const errors = [];
    let isValid = true;

    if (!email) {
        addError('email', 'emailError', 'Email address is required');
        errors.push('Email address is required');
        isValid = false;
    } else if (!isValidEmail(email)) {
        addError('email', 'emailError', 'Invalid email format');
        errors.push('Invalid email format');
        isValid = false;
    }

    if (!password) {
        addError('password', 'passwordError', 'Password is required');
        errors.push('Password is required');
        isValid = false;
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    form.submit();
}
/* ============================================
   EDIT PROFILE VALIDATION (USER)
   ============================================ */

function validateEditProfile(event) {
    event.preventDefault();
    clearAllErrors();
    
    const userName = document.getElementById('userName').value.trim();
    const password = document.getElementById('password').value;
    
    const errors = [];
    let isValid = true;

    if (!userName) {
        addError('userName', 'userNameError', 'Full name is required');
        errors.push('Full name is required');
        isValid = false;
    } else if (userName.length < 3) {
        addError('userName', 'userNameError', 'Full name must be at least 3 characters');
        errors.push('Full name must be at least 3 characters');
        isValid = false;
    }

    if (password && password.length < 6) {
        addError('password', 'passwordError', 'New password must be at least 6 characters');
        errors.push('New password must be at least 6 characters');
        isValid = false;
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    event.target.submit();
    return false;
}

/* ============================================
   EDIT ADMIN PROFILE VALIDATION
   ============================================ */

function validateAdminProfile(event) {
    event.preventDefault();
    clearAllErrors();
    
    const adminName = document.getElementById('adminName').value.trim();
    const adminPassword = document.getElementById('adminPassword').value;
    
    const errors = [];
    let isValid = true;

    if (!adminName) {
        addError('adminName', 'adminNameError', 'Full name is required');
        errors.push('Full name is required');
        isValid = false;
    } else if (adminName.length < 3) {
        addError('adminName', 'adminNameError', 'Full name must be at least 3 characters');
        errors.push('Full name must be at least 3 characters');
        isValid = false;
    }

    if (adminPassword && adminPassword.length < 6) {
        addError('adminPassword', 'adminPasswordError', 'New password must be at least 6 characters');
        errors.push('New password must be at least 6 characters');
        isValid = false;
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    event.target.submit();
    return false;
}

/* ============================================
   REPORT / EDIT ITEM VALIDATION
   ============================================ */

function validateReportItem(event) {
    event.preventDefault();
    clearAllErrors();
    
    const title = document.getElementById('itemTitle').value.trim();
    const category = document.getElementById('itemCategory').value;
    const location = document.getElementById('itemLocation').value.trim();
    const description = document.getElementById('itemDescription').value.trim();
    const dateInput = document.getElementById('itemDate');
    
    const errors = [];
    let isValid = true;

    if (!title) {
        addError('itemTitle', 'itemTitleError', 'Item title is required');
        errors.push('Item title is required');
        isValid = false;
    }
    
    if (!category) {
        addError('itemCategory', 'itemCategoryError', 'Please select a category');
        errors.push('Please select a category');
        isValid = false;
    }
    
    if (!location) {
        addError('itemLocation', 'itemLocationError', 'Location is required');
        errors.push('Location is required');
        isValid = false;
    }
    
    if (!description) {
        addError('itemDescription', 'itemDescriptionError', 'Description is required');
        errors.push('Description is required');
        isValid = false;
    }
    
    // Date validation (if present)
    if (dateInput) {
        const date = dateInput.value;
        if (!date) {
            addError('itemDate', 'itemDateError', 'Date is required');
            errors.push('Date is required');
            isValid = false;
        } else {
            const selectedDate = new Date(date);
            const today = new Date();
            if (selectedDate > today) {
                addError('itemDate', 'itemDateError', 'Date cannot be in the future');
                errors.push('Date cannot be in the future');
                isValid = false;
            }
        }
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    event.target.submit();
    return false;
}
