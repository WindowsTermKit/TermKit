var termkit = {
  version: 1,
};
print(typeof require, require);
print(typeof require.paths, require.paths);
print(typeof require.paths.unshift, require.paths.unshift);

//require.paths.unshift('./socket.io-node/lib');
//require.paths.unshift('.');
//require.paths.unshift('../Shared/');
var util = require('util');
print(typeof util, util);
print(typeof util.log, util.log);
print(typeof util.debug, util.debug);
util.log("It works!");
util.debug("something");
util.debug(4.5);
util.debug("something");

var fs = require('fs');
file = fs.open("test.txt");
util.log(fs.readable);
util.log(fs.writable);
file.on('data', function(msg) {
    util.log(msg);
});
file.on('close', function() {
    util.log("File closed.");
});
