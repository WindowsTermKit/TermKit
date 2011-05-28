var view = require('view/view'),
    expandPath = require('misc').expandPath;

exports.main = function (tokens, pipes, exit) {

  var out = new view.bridge(pipes.viewOut);
  
  // Validate syntax.
  if (tokens.length > 2) {
    out.print('Usage: cd [dir]');
    return exit(false);
  }
  var path = null;
  
  // Check what default path we should change to if no arguments
  // were provided.
  var oldPath = process.cwd();
  var wasWindows = false;
  if (oldPath.substring(0, "/cygdrive/".length) == "/cygdrive/")
  {
    wasWindows = true;
    path = tokens[1] || oldPath.substring(0, "/cygdrive/".length + 1);
  }
  else
    path = tokens[1] || '~';
  
  // Complete path
  expandPath(path, function (path) {
    // Try to change working dir.
    try {
      process.chdir(path);
      
      // Ensure that in Windows we remain in a drive at all times.
      if (wasWindows && process.cwd().substring(0, "/cygdrive/".length) != "/cygdrive/") {
        out.print('You can\'t move higher than the top-level directory.');
        process.chdir(oldPath.substring(0, "/cygdrive/".length + 1));
      }
    }
    catch (error) {
      out.print(error.message + ' (' + path + ')');
      return exit(false);
    }

    exit(true);
  }); // expandPath
};
