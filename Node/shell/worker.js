require.paths.unshift('.');
require.paths.unshift('..');
require.paths.unshift(__dirname);
require.paths.unshift(__dirname + '/..');

var processor = require('processor');

// Change to home directory.
if (/* TODO: Check if /cygdrive/ exists */ true)
    process.chdir("/cygdrive/c");
else
    process.chdir(process.env.HOME);

// Set up processor.
var p = new processor.processor(process.openStdin(), process.stdout);
