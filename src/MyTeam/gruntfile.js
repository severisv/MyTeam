/// <binding AfterBuild='less:development' ProjectOpened='bower:install, watch, watch:less' />
// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "wwwroot/lib",
                    layout: "byComponent",
                    cleanTargetDir: false
                }
            }
        },
        less: {
            development: {
                options: {
                    paths: ["Assets"]
                },
                files: { "wwwroot/css/site.css": "assets/site.less" }
            }
        },
        watch: {
            less: {
                files: ["assets/*.less"],
                tasks: ["less"],
                options: {
                    livereload: true
                }
            }
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
    grunt.registerTask("default", ["bower:install"]);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
    grunt.loadNpmTasks("grunt-contrib-less");
    grunt.loadNpmTasks("grunt-contrib-watch");
};