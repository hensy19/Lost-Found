/* ============================================
   REGISTER FORM VALIDATION
   ============================================ */

// Handle role selection
function selectRole(button) {
    document.querySelectorAll('.role-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    button.classList.add('active');
    document.getElementById('roleInput').value = button.getAttribute('data-role');
}

// Main form validation
function validateForm(event) {
    event.preventDefault();
    clearAllErrors();
    
    const fullName = document.getElementById('fullName').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const terms = document.getElementById('terms').checked;
    
    const errors = [];
    let isValid = true;

    // Validate Full Name
    if (!fullName) {
        addError('fullName', 'fullNameError', 'Full name is required');
        errors.push('Full name is required');
        isValid = false;
    } else if (fullName.length < 3) {
        addError('fullName', 'fullNameError', 'Full name must be at least 3 characters');
        errors.push('Full name must be at least 3 characters');
        isValid = false;
    }

    // Validate Email
    if (!email) {
        addError('email', 'emailError', 'Email address is required');
        errors.push('Email address is required');
        isValid = false;
    } else if (!isValidEmail(email)) {
        addError('email', 'emailError', 'Invalid email format');
        errors.push('Invalid email format');
        isValid = false;
    }

    // Validate Password
    if (!password) {
        addError('password', 'passwordError', 'Password is required');
        errors.push('Password is required');
        isValid = false;
    } else if (password.length < 6) {
        addError('password', 'passwordError', 'Password must be at least 6 characters');
        errors.push('Password must be at least 6 characters');
        isValid = false;
    }

    // Validate Confirm Password
    if (!confirmPassword) {
        addError('confirmPassword', 'confirmPasswordError', 'Confirm password is required');
        errors.push('Confirm password is required');
        isValid = false;
    } else if (password !== confirmPassword) {
        addError('confirmPassword', 'confirmPasswordError', 'Passwords do not match');
        errors.push('Passwords do not match');
        isValid = false;
    }

    // Validate Terms
    if (!terms) {
        document.getElementById('termsError').textContent = 'You must agree to the Community Guidelines';
        errors.push('You must agree to the Community Guidelines');
        isValid = false;
    }

    if (!isValid) {
        displayErrors(errors);
        window.scrollTo(0, 0);
        return false;
    }

    document.querySelector('form').submit();
    return false;
}

/* ============================================
   LOGIN FORM VALIDATION
   ============================================ */

function validateLoginForm(event) {
    event.preventDefault();
    clearAllErrors();
    
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    
    const errors = [];
    let isValid = true;

    // Validate Email
    if (!email) {
        addError('email', 'emailError', 'Email address is required');
        errors.push('Email address is required');
        isValid = false;
    } else if (!isValidEmail(email)) {
        addError('email', 'emailError', 'Invalid email format');
        errors.push('Invalid email format');
        isValid = false;
    }

    // Validate Password
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

    document.querySelector('form').submit();
    return false;
}

// Validate email format
function isValidEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// Add error to field
function addError(fieldId, errorId, message) {
    const field = document.getElementById(fieldId);
    const errorDisplay = document.getElementById(errorId);
    if (field) field.classList.add('is-invalid');
    if (errorDisplay) errorDisplay.textContent = message;
}

// Clear all errors
function clearAllErrors() {
    document.getElementById('validationErrors').style.display = 'none';
    document.getElementById('errorList').innerHTML = '';
    document.querySelectorAll('.form-control.is-invalid').forEach(field => {
        field.classList.remove('is-invalid');
    });
    document.querySelectorAll('.input-error-text').forEach(error => {
        error.textContent = '';
    });
}

// Display error summary
function displayErrors(errorArray) {
    const errorContainer = document.getElementById('validationErrors');
    const errorList = document.getElementById('errorList');
    errorList.innerHTML = '';
    errorArray.forEach(error => {
        const li = document.createElement('li');
        li.textContent = error;
        errorList.appendChild(li);
    });
    errorContainer.style.display = 'block';
}

// Clear errors on input
document.addEventListener('input', function(e) {
    if (e.target.classList.contains('form-control')) {
        e.target.classList.remove('is-invalid');
    }
});

// Clear terms error on check
document.addEventListener('change', function(e) {
    if (e.target.id === 'terms') {
        document.getElementById('termsError').textContent = '';
    }
});
