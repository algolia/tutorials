$(document).ready(function () {
    var algolia = new AlgoliaSearch('<APPLICATION_ID>', '<SEARCH_ONLY_API_KEY>');
    var index = algolia.initIndex('packages');

    var template = Hogan.compile(
        '<a href="/home/details/{{{ Id }}}">' +
            '<div class="hit">' +
                '<div class="name">' +
                    '{{{ Name }}} ' +
                '</div>' +
                '{{#attributes}}' +
                '<div class="attribute">{{ attribute }}: {{{ value }}}</div>' +
                '{{/attributes}}' +
            '</div>' +
        '</a>');

    $('#typeahead-algolia').typeahead({
        highlight: false,
        hint: true,
        minLength: 1
    },
    {
        source: index.ttAdapter({ "hitsPerPage": 10 }),
        displayKey: 'Name',
        templates: {
            suggestion: function (hit) {
                // select matching attributes only
                hit.attributes = [];
                for (var attribute in hit._highlightResult) {
                    if (attribute === 'Name') {
                        // already handled by the template
                        continue;
                    }
                    // all others attributes that are matching should be added in the attributes array
                    // so we can display them in the dropdown menu. Non-matching attributes are skipped.
                    if (hit._highlightResult[attribute].matchLevel !== 'none') {
                        hit.attributes.push({ attribute: attribute, value: hit._highlightResult[attribute].value });
                    }
                }

                // render the hit using Hogan.js
                return template.render(hit);
            }
        }
    });
});