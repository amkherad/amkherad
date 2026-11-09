---
name: resume-build
description: Build, preview, and publish this Jekyll resume, including the GitHub Pages workflow that renders _data/data.yml to HTML and then to AliKherad-Resume.pdf. Use when changing _config.yml, layouts, Sass/CSS, the PDF pipeline, the inline-html script, or debugging a failing deploy or a PDF that differs from the site.
metadata:
  short-description: Build, preview, and publish the resume
---

# Resume Build and Publish

## How the site is served

The theme is remote (`remote_theme: sharu725/online-cv`), so layouts and includes are fetched at build time and are not in this repo. Locally there are only two entry points, `index.html` (layout `default`) and `print.html` (layout `print`, permalink `/print`), plus style overrides.

`baseurl` is `/amkherad`, so the public site lives at `https://amkherad.ir/amkherad`, the print view at `https://amkherad.ir/amkherad/print`, and the PDF at `https://amkherad.ir/amkherad/AliKherad-Resume.pdf`. The custom domain serves from the repo root while GitHub Pages still mounts the project under the repository path.

Styles are layered: `_sass/default.scss` is a local copy that shadows the theme's entry point and appends `@import "custom"` after the theme's imports. That import is the hook — deleting or simplifying `_sass/default.scss` drops every rule in `_sass/custom.scss`. Put overrides in `_sass/custom.scss`, not inline in the HTML.

There is currently no local JavaScript. If you add one, place it under `assets/` and reference it with a relative `src` so the inliner can pick it up.

## The GitHub Pages pipeline

`.github/workflows/jekyll.yml` runs on push to `main` (and manually). Steps, in order:

1. Jekyll build with `actions/jekyll-build-pages` into `_site`.
2. Copy `_site` to `output`, copy Font Awesome webfonts to `output/webfonts`, strip the `"/amkherad/` prefix from `index.html` and `print.html`, and rewrite `../webfonts` to `webfonts`.
3. Inline CSS and JS in both pages via `scripts/inline-html.sh`.
4. Bump `14px` to `15px` in the inlined print page.
5. Move the web `index.html` aside and promote the inlined print page over it.
6. Render `./output` to `./output/AliKherad-Resume.pdf` with `ferdinandkeller/html-to-pdf-action`.
7. Rebuild Jekyll into `_site`, copy it plus the PDF into `dist/`, upload and deploy.

The swap in steps 5–6 is why the PDF looks like `print.html` and not like the site. `index.html.bk` holds the discarded web page and is never restored because the second Jekyll build regenerates `_site` from source.

Hard-coded couplings — change these together or the deploy produces a wrong or 404 artifact:

- The `sed` that strips `"/amkherad/` must match `baseurl` in `_config.yml`.
- The output filename `AliKherad-Resume.pdf` must match `sidebar.pdf` in `_data/data.yml`.
- `_site/assets/plugins/font-awesome/webfonts/` must exist in the theme; a theme bump that moves its assets fails this `cp`.

The workflow steps run under `bash -e`, so a failed `cp`, `sed` or script aborts the job. When a deploy fails, read the "Setup PDF generation" step first — that is where path assumptions break.

`scripts/inline-html.sh` is bash with `set -e`, uses `mktemp`, and only inlines `<link rel="stylesheet" href="...">` and `<script src="...">` whose target is a local file. It leaves other tags untouched. On Windows run it through Git Bash or WSL.

The matching is line-based and strict: the stylesheet tag must have `rel="stylesheet"` before `href`, and the script tag must have `src` and `</script>` on the same line. A reformatted tag is not inlined and does not fail the build, so the PDF can silently ship with an external reference. Keep those tags in that form, or check the `Setup PDF generation` output for `Inlining` lines after any layout change.

The `14px` to `15px` substitution is a blunt global replace across the inlined print page, so it also hits any unrelated `14px` declaration. It exists to make the PDF text a touch larger; prefer adjusting print styles in `_sass/custom.scss` when you can.

## Print and PDF specifics

Page breaks come from `<div class="page-break">` markers in `print.html` and `index.html`, styled in `_sass/custom.scss` with `break-before: page`. Reordering or adding sections means touching both HTML files, otherwise the site and the PDF diverge.

`_sass/custom.scss` also justifies text, adds RTL list indentation, zeroes time-badge padding in print, and hides `.print-button` / `.pdf-button`. Buttons are hidden rather than removed, so they cost nothing in the PDF.

## Docker and Drone

`Dockerfile` builds `_site` with the `jekyll/jekyll` image and serves it via nginx; `nginx.conf` handles the `/amkherad` prefix. `.drone/drone-ci.yml` builds and deploys that image to a home server. This path publishes the site only — it does not generate the PDF, and it is not the public deployment. GitHub Pages is.

## Building locally

Ruby, Bundler and Jekyll are not installed on this machine, so a plain `bundle exec jekyll serve` will fail here. Two workable routes:

```bash
# With Ruby available
bundle install
bundle exec jekyll serve --baseurl ""
```

```bash
# With Docker running; theme is fetched from GitHub, so network is required
docker run --rm -it -v "$(pwd):/srv/jekyll" -p 4000:4000 jekyll/jekyll:latest \
  jekyll serve --host 0.0.0.0 --baseurl ""
```

Passing `--baseurl ""` makes local links resolve; production keeps `/amkherad`. Drop it to reproduce the deployed URL shape.

There is no local equivalent of the PDF step. To check print output, open `/print` and use the browser's print preview at A4 with backgrounds enabled.

## Verifying a change

Parse the content file and confirm the Liquid templates still include what you expect:

```bash
python -c "import yaml; yaml.safe_load(open('_data/data.yml',encoding='utf-8')); print('yaml ok')"
diff index.html print.html
```

Before pushing, grep for the strings the pipeline depends on whenever you touch config, filenames or theme assets. `rg` is not installed on the Windows machine, so use `git grep`:

```bash
git grep -n -E 'amkherad/|AliKherad-Resume|webfonts' -- .github _config.yml _data
```

If a build changes the PDF's page count or drops a section, compare `print.html` to `index.html` first, then check whether the corresponding key in `_data/data.yml` is still enabled.
