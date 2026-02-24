#!/bin/sh

# Create the runtime config file from environment variables
echo "window.__RUNTIME_CONFIG__ = {" > /usr/share/nginx/html/env-config.js

# Map environment variables to the runtime config object
# Check both VITE_ prefixed and non-prefixed variables
[ -n "$VITE_API_BASE_URL" ] && API_BASE_URL=$VITE_API_BASE_URL
[ -n "$VITE_GOOGLE_CLIENT_ID" ] && GOOGLE_CLIENT_ID=$VITE_GOOGLE_CLIENT_ID
[ -n "$VITE_USE_MOCK_DATA" ] && USE_MOCK_DATA=$VITE_USE_MOCK_DATA

[ -n "$API_BASE_URL" ] && echo "  VITE_API_BASE_URL: \"$API_BASE_URL\"," >> /usr/share/nginx/html/env-config.js
[ -n "$GOOGLE_CLIENT_ID" ] && echo "  VITE_GOOGLE_CLIENT_ID: \"$GOOGLE_CLIENT_ID\"," >> /usr/share/nginx/html/env-config.js
[ -n "$USE_MOCK_DATA" ] && echo "  VITE_USE_MOCK_DATA: \"$USE_MOCK_DATA\"," >> /usr/share/nginx/html/env-config.js

echo "};" >> /usr/share/nginx/html/env-config.js

# Start nginx
exec "$@"
