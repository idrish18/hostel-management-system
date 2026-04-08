// Swagger Bearer Token Support Script
// This script adds JWT Bearer token input field and automatically injects it into requests

(function() {
    // Wait for Swagger UI to fully load
    const waitForSwaggerUI = setInterval(function() {
        const topbar = document.querySelector('.topbar');
        if (topbar && !document.querySelector('#bearer-token-container')) {
            clearInterval(waitForSwaggerUI);
            setupBearerTokenUI();
        }
    }, 100);

    function setupBearerTokenUI() {
        // Create container for bearer token input
        const container = document.createElement('div');
        container.id = 'bearer-token-container';
        container.style.cssText = `
            padding: 15px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-bottom: 2px solid #555;
            margin-bottom: 0;
        `;

        // Title
        const title = document.createElement('h3');
        title.textContent = '🔐 JWT Bearer Token Authentication';
        title.style.cssText = 'margin-top: 0; margin-bottom: 10px; font-size: 14px;';

        // Token input container
        const inputContainer = document.createElement('div');
        inputContainer.style.cssText = 'display: flex; gap: 10px; flex-wrap: wrap; align-items: center;';

        // Token input field
        const input = document.createElement('input');
        input.type = 'password';
        input.id = 'jwtBearerTokenInput';
        input.placeholder = 'Paste your JWT token here...';
        input.style.cssText = `
            flex: 1;
            min-width: 250px;
            padding: 8px 12px;
            border: none;
            border-radius: 4px;
            font-family: monospace;
            font-size: 12px;
        `;

        // Set token button
        const setButton = document.createElement('button');
        setButton.textContent = 'Set Token';
        setButton.style.cssText = `
            padding: 8px 16px;
            background: #4CAF50;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: bold;
            transition: background 0.3s;
        `;
        setButton.onmouseover = () => setButton.style.background = '#45a049';
        setButton.onmouseout = () => setButton.style.background = '#4CAF50';
        setButton.onclick = function() {
            const token = input.value.trim();
            if (!token) {
                alert('❌ Please enter a valid JWT token');
                return;
            }
            localStorage.setItem('jwtBearerToken', token);
            injectTokenIntoRequests(token);
            alert('✅ Bearer token set successfully!');
        };

        // Clear token button
        const clearButton = document.createElement('button');
        clearButton.textContent = 'Clear';
        clearButton.style.cssText = `
            padding: 8px 12px;
            background: #f44336;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: background 0.3s;
        `;
        clearButton.onmouseover = () => clearButton.style.background = '#da190b';
        clearButton.onmouseout = () => clearButton.style.background = '#f44336';
        clearButton.onclick = function() {
            input.value = '';
            localStorage.removeItem('jwtBearerToken');
            delete window.jwtBearerToken;
            alert('⚠️ Token cleared');
        };

        // Info section
        const info = document.createElement('div');
        info.style.cssText = 'margin-top: 10px; font-size: 12px; color: #e0e0e0; background: rgba(0,0,0,0.2); padding: 8px; border-radius: 4px;';
        info.innerHTML = `
            <strong>📝 How to get a token:</strong><br>
            1. Scroll down to <code>POST /api/auth/login</code><br>
            2. Use credentials: <code>admin@hms.com</code> / <code>Admin@123</code><br>
            3. Copy the <code>token</code> from the response<br>
            4. Paste it above and click "Set Token"<br>
            5. All subsequent requests will include your Bearer token!
        `;

        // Assemble the UI
        inputContainer.appendChild(input);
        inputContainer.appendChild(setButton);
        inputContainer.appendChild(clearButton);

        container.appendChild(title);
        container.appendChild(inputContainer);
        container.appendChild(info);

        // Insert at the beginning of the page
        const swaggerUI = document.querySelector('.swagger-ui');
        if (swaggerUI) {
            swaggerUI.insertBefore(container, swaggerUI.firstChild);
        }

        // Check if token exists in localStorage and load it
        const savedToken = localStorage.getItem('jwtBearerToken');
        if (savedToken) {
            input.value = '••••••••' + savedToken.substring(savedToken.length - 8);
            injectTokenIntoRequests(savedToken);
        }
    }

    function injectTokenIntoRequests(token) {
        window.jwtBearerToken = token;

        // Intercept fetch requests
        const originalFetch = window.fetch;
        window.fetch = function(resource, config = {}) {
            if (window.jwtBearerToken) {
                config.headers = config.headers || {};
                config.headers['Authorization'] = `Bearer ${window.jwtBearerToken}`;
            }
            return originalFetch.apply(this, arguments);
        };

        // Intercept XMLHttpRequest
        const originalOpen = XMLHttpRequest.prototype.open;
        XMLHttpRequest.prototype.open = function(method, url, ...rest) {
            this._requestURL = url;
            return originalOpen.apply(this, [method, url, ...rest]);
        };

        const originalSetRequestHeader = XMLHttpRequest.prototype.setRequestHeader;
        XMLHttpRequest.prototype.setRequestHeader = function(header, value) {
            if (window.jwtBearerToken && header.toLowerCase() === 'authorization') {
                return originalSetRequestHeader.apply(this, ['Authorization', `Bearer ${window.jwtBearerToken}`]);
            }
            return originalSetRequestHeader.apply(this, [header, value]);
        };

        const originalSend = XMLHttpRequest.prototype.send;
        XMLHttpRequest.prototype.send = function(body) {
            if (window.jwtBearerToken && this.getRequestHeader('Authorization') === null) {
                this.setRequestHeader('Authorization', `Bearer ${window.jwtBearerToken}`);
            }
            return originalSend.apply(this, arguments);
        };
    }
})();
