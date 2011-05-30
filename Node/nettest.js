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
util.log("It works!");
