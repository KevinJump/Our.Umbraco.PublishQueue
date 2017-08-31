/*
    copy the app_plugins folder when it changes
    means we don't have to rebuild, and umbraco
    loads the changes quicker.
*/
var gulp = require('gulp');

var sources = [
    './Our.Umbraco.PublishQueue/App_Plugins'
];

var dest = './PublishQueue.Web/App_Plugins';

gulp.task('simple-watch', function () {

    for (var i = 0; i < sources.length; i++) {
        console.log('watch: ' + sources[i] + "/**/*");
        watchy(sources[i]);
    }
});

function watchy(source) {

    gulp.watch(source + '/**/*', function (event) {
        if (event.type === 'changed') {
            console.dir("File: " + event.path);
            gulp.src(event.path, { "base": source })
                .pipe(gulp.dest(dest));
        }
    });
}

gulp.task('default', ['simple-watch'])