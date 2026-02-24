const fs = require('fs');

const files = ['specs.json', 'specs2.json'];
const allSchemas = {};

files.forEach(file => {
    try {
        const content = fs.readFileSync(file, 'utf8');
        const json = JSON.parse(content);
        if (json.components && json.components.schemas) {
            allSchemas[file] = Object.keys(json.components.schemas);
        } else {
            allSchemas[file] = [];
        }
    } catch (e) {
        console.error(`Error reading ${file}:`, e.message);
        allSchemas[file] = [];
    }
});

console.log(JSON.stringify(allSchemas, null, 2));
