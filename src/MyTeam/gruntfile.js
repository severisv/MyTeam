/// <binding AfterBuild='watch' ProjectOpened='bower:install, watch' />

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
                files: {
                    "wwwroot/css/site.css": "assets/site.less",
                    "wwwroot/css/bootstrap-override.css": "assets/bootstrap-override.less"
                }
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

    grunt.registerTask("default", ["bower:install"]);

    grunt.loadNpmTasks("grunt-bower-task");
    grunt.loadNpmTasks("grunt-contrib-less");
    grunt.loadNpmTasks("grunt-contrib-watch");
};