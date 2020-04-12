Auto DevOps



Introduced in GitLab 10.0.
Generally available on GitLab 11.0.


Auto DevOps provides pre-defined CI/CD configuration which allows you to automatically detect, build, test,
deploy, and monitor your applications. Leveraging CI/CD best practices and tools, Auto DevOps aims
to simplify the setup and execution of a mature & modern software development lifecycle.

Overview
With Auto DevOps, the software development process becomes easier to set up
as every project can have a complete workflow from verification to monitoring
with minimal configuration. Just push your code and GitLab takes
care of everything else. This makes it easier to start new projects and brings
consistency to how applications are set up throughout a company.
For an introduction to Auto DevOps, watch AutoDevOps in GitLab 11.0.

Enabled by default
Starting with GitLab 11.3, the Auto DevOps pipeline is enabled by default for all
projects. If it has not been explicitly enabled for the project, Auto DevOps will be automatically
disabled on the first pipeline failure. Your project will continue to use an alternative
CI/CD configuration file if one is found. A GitLab
administrator can change this setting
in the admin area.

Quick start
If you are using GitLab.com, see the quick start guide
for how to use Auto DevOps with GitLab.com and a Kubernetes cluster on Google Kubernetes
Engine (GKE).
If you are using a self-hosted instance of GitLab, you will need to configure the
Google OAuth2 OmniAuth Provider before
you can configure a cluster on GKE. Once this is set up, you can follow the steps on the
quick start guide to get started.

Comparison to application platforms and PaaS
Auto DevOps provides functionality that is often included in an application
platform or a Platform as a Service (PaaS). It takes inspiration from the
innovative work done by Heroku and goes beyond it
in multiple ways:

Auto DevOps works with any Kubernetes cluster; you're not limited to running
on GitLab's infrastructure. (Note that many features also work without Kubernetes).
There is no additional cost (no markup on the infrastructure costs), and you
can use a self-hosted Kubernetes cluster or Containers as a Service on any
public cloud (for example, Google Kubernetes Engine).
Auto DevOps has more features including security testing, performance testing,
and code quality testing.
Auto DevOps offers an incremental graduation path. If you need advanced customizations,
you can start modifying the templates without having to start over on a
completely different platform. Review the customizing section for more information.


Features
Comprised of a set of stages, Auto DevOps brings these best practices to your
project in a simple and automatic way:

Auto Build
Auto Test

Auto Code Quality (STARTER)


Auto SAST (Static Application Security Testing) (ULTIMATE)


Auto Dependency Scanning (ULTIMATE)


Auto License Compliance (ULTIMATE)


Auto Container Scanning (ULTIMATE)

Auto Review Apps

Auto DAST (Dynamic Application Security Testing) (ULTIMATE)

Auto Deploy

Auto Browser Performance Testing (PREMIUM)

Auto Monitoring

As Auto DevOps relies on many different components, it's good to have a basic
knowledge of the following:

Kubernetes
Helm
Docker
GitLab Runner
Prometheus

Auto DevOps provides great defaults for all the stages; you can, however,
customize almost everything to your needs.
For an overview on the creation of Auto DevOps, read the blog post From 2/3 of the Self-Hosted Git Market, to the Next-Generation CI System, to Auto DevOps.
NOTE: Note
Kubernetes clusters can be used without
Auto DevOps.

Requirements
To make full use of Auto DevOps, you will need:


Kubernetes (for Auto Review Apps, Auto Deploy, and Auto Monitoring)
To enable deployments, you will need:


A Kubernetes 1.12+ cluster for the project. The easiest
way is to add a new cluster using the GitLab UI.


NGINX Ingress. You can deploy it to your Kubernetes cluster by installing
the GitLab-managed app for Ingress,
once you have configured GitLab's Kubernetes integration in the previous step.
Alternatively, you can use the
nginx-ingress
Helm chart to install Ingress manually.
NOTE: Note:
If you are using your own Ingress instead of the one provided by GitLab's managed
apps, ensure you are running at least version 0.9.0 of NGINX Ingress and
enable Prometheus metrics
in order for the response metrics to appear. You will also have to
annotate
the NGINX Ingress deployment to be scraped by Prometheus using
prometheus.io/scrape: "true" and prometheus.io/port: "10254".




Base domain (for Auto Review Apps, Auto Deploy, and Auto Monitoring)
You will need a domain configured with wildcard DNS which is going to be used
by all of your Auto DevOps applications. If you're using the
GitLab-managed app for Ingress,
the URL endpoint will be automatically configured for you.
You will then need to specify the Auto DevOps base domain.


GitLab Runner (for all stages)
Your Runner needs to be configured to be able to run Docker. Generally this
means using either the Docker
or Kubernetes executors, with
privileged mode enabled.
The Runners do not need to be installed in the Kubernetes cluster, but the
Kubernetes executor is easy to use and is automatically autoscaling.
Docker-based Runners can be configured to autoscale as well, using Docker
Machine.
If you have configured GitLab's Kubernetes integration in the first step, you
can deploy it to your cluster by installing the
GitLab-managed app for GitLab Runner.
Runners should be registered as shared Runners
for the entire GitLab instance, or specific Runners
that are assigned to specific projects (the default if you have installed the
GitLab Runner managed application).


Prometheus (for Auto Monitoring)
To enable Auto Monitoring, you will need Prometheus installed somewhere
(inside or outside your cluster) and configured to scrape your Kubernetes cluster.
If you have configured GitLab's Kubernetes integration, you can deploy it to
your cluster by installing the
GitLab-managed app for Prometheus.
The Prometheus service
integration needs to be enabled for the project (or enabled as a
default service template
for the entire GitLab installation).
To get response metrics (in addition to system metrics), you need to
configure Prometheus to monitor NGINX.


If you do not have Kubernetes or Prometheus installed, then Auto Review Apps,
Auto Deploy, and Auto Monitoring will be silently skipped.
One all requirements are met, you can go ahead and enable Auto DevOps.

Auto DevOps base domain
The Auto DevOps base domain is required if you want to make use of
Auto Review Apps, Auto Deploy, and
Auto Monitoring. It can be defined in any of the following
places:

either under the cluster's settings, whether for projects or groups

or in instance-wide settings in the admin area > Settings under the "Continuous Integration and Delivery" section
or at the project level as a variable: KUBE_INGRESS_BASE_DOMAIN

or at the group level as a variable: KUBE_INGRESS_BASE_DOMAIN.

The base domain variable KUBE_INGRESS_BASE_DOMAIN follows the same order of precedence
as other environment variables.
TIP: Tip:
If you're using the GitLab managed app for Ingress,
the URL endpoint should be automatically configured for you. All you have to do
is use its value for the KUBE_INGRESS_BASE_DOMAIN variable.
NOTE: Note:
AUTO_DEVOPS_DOMAIN was deprecated in GitLab 11.8
and replaced with KUBE_INGRESS_BASE_DOMAIN. It was removed in
GitLab 12.0.
A wildcard DNS A record matching the base domain(s) is required, for example,
given a base domain of example.com, you'd need a DNS entry like:
*.example.com   3600     A     1.2.3.4
In this case, example.com is the domain name under which the deployed apps will be served,
and 1.2.3.4 is the IP address of your load balancer; generally NGINX
(see requirements). How to set up the DNS record is beyond
the scope of this document; you should check with your DNS provider.
Alternatively you can use free public services like nip.io
which provide automatic wildcard DNS without any configuration. Just set the
Auto DevOps base domain to 1.2.3.4.nip.io.
Once set up, all requests will hit the load balancer, which in turn will route
them to the Kubernetes pods that run your application(s).

Enabling/Disabling Auto DevOps
When first using Auto DevOps, review the requirements to ensure all necessary components to make
full use of Auto DevOps are available. If this is your fist time, we recommend you follow the
quick start guide.
GitLab.com users can enable/disable Auto DevOps at the project-level only. Self-managed users
can enable/disable Auto DevOps at the project-level, group-level or instance-level.

At the project level
If enabling, check that your project doesn't have a .gitlab-ci.yml, or if one exists, remove it.

Go to your project's Settings > CI/CD > Auto DevOps.
Toggle the Default to Auto DevOps pipeline checkbox (checked to enable, unchecked to disable)
When enabling, it's optional but recommended to add in the base domain
that will be used by Auto DevOps to deploy your application
and choose the deployment strategy.
Click Save changes for the changes to take effect.

When the feature has been enabled, an Auto DevOps pipeline is triggered on the default branch.

At the group level

Introduced in GitLab 11.10.

Only administrators and group owners can enable or disable Auto DevOps at the group level.
To enable or disable Auto DevOps at the group-level:

Go to group's Settings > CI/CD > Auto DevOps page.
Toggle the Default to Auto DevOps pipeline checkbox (checked to enable, unchecked to disable).
Click Save changes button for the changes to take effect.

When enabling or disabling Auto DevOps at group-level, group configuration will be implicitly used for
the subgroups and projects inside that group, unless Auto DevOps is specifically enabled or disabled on
the subgroup or project.

At the instance level (Administrators only)
Even when disabled at the instance level, group owners and project maintainers can still enable
Auto DevOps at the group and project level, respectively.

Go to Admin area > Settings > Continuous Integration and Deployment.
Toggle the checkbox labeled Default to Auto DevOps pipeline for all projects.
If enabling, optionally set up the Auto DevOps base domain which will be used for Auto Deploy and Auto Review Apps.
Click Save changes for the changes to take effect.


Enable for a percentage of projects
There is also a feature flag to enable Auto DevOps by default to your chosen percentage of projects.
This can be enabled from the console with the following, which uses the example of 10%:
Feature.get(:force_autodevops_on_by_default).enable_percentage_of_actors(10)

Deployment strategy

Introduced in GitLab 11.0.

You can change the deployment strategy used by Auto DevOps by going to your
project's Settings > CI/CD > Auto DevOps.
The available options are:


Continuous deployment to production: Enables Auto Deploy
with master branch directly deployed to production.


Continuous deployment to production using timed incremental rollout: Sets the
INCREMENTAL_ROLLOUT_MODE variable
to timed, and production deployment will be executed with a 5 minute delay between
each increment in rollout.


Automatic deployment to staging, manual deployment to production: Sets the
STAGING_ENABLED and
INCREMENTAL_ROLLOUT_MODE variables
to 1 and manual. This means:


master branch is directly deployed to staging.
Manual actions are provided for incremental rollout to production.




Using multiple Kubernetes clusters (PREMIUM)

When using Auto DevOps, you may want to deploy different environments to
different Kubernetes clusters. This is possible due to the 1:1 connection that
exists between them.
In the Auto DevOps template (used behind the scenes by Auto DevOps), there
are currently 3 defined environment names that you need to know:


review/ (every environment starting with review/)
staging
production

Those environments are tied to jobs that use Auto Deploy, so
except for the environment scope, they would also need to have a different
domain they would be deployed to. This is why you need to define a separate
KUBE_INGRESS_BASE_DOMAIN variable for all the above
based on the environment.
The following table is an example of how the three different clusters would
be configured.



Cluster name
Cluster environment scope

KUBE_INGRESS_BASE_DOMAIN variable value
Variable environment scope
Notes




review
review/*
review.example.com
review/*
The review cluster which will run all Review Apps. * is a wildcard, which means it will be used by every environment name starting with review/.


staging
staging
staging.example.com
staging
(Optional) The staging cluster which will run the deployments of the staging environments. You need to enable it first.


production
production
example.com
production
The production cluster which will run the deployments of the production environment. You can use incremental rollouts.



To add a different cluster for each environment:


Navigate to your project's Operations > Kubernetes and create the Kubernetes clusters
with their respective environment scope as described from the table above.



After the clusters are created, navigate to each one and install Helm Tiller
and Ingress. Wait for the Ingress IP address to be assigned.


Make sure you have configured your DNS with the
specified Auto DevOps domains.


Navigate to each cluster's page, through Operations > Kubernetes,
and add the domain based on its Ingress IP address.


Now that all is configured, you can test your setup by creating a merge request
and verifying that your app is deployed as a review app in the Kubernetes
cluster with the review/* environment scope. Similarly, you can check the
other environments.

Stages of Auto DevOps
The following sections describe the stages of Auto DevOps. Read them carefully
to understand how each one works.

Auto Build
Auto Build creates a build of the application using an existing Dockerfile or
Heroku buildpacks.
Either way, the resulting Docker image is automatically pushed to the
Container Registry and tagged with the commit SHA or tag.

Auto Build using a Dockerfile
If a project's repository contains a Dockerfile, Auto Build will use
docker build to create a Docker image.
If you are also using Auto Review Apps and Auto Deploy and choose to provide
your own Dockerfile, make sure you expose your application to port
5000 as this is the port assumed by the
default Helm chart. Alternatively you can override the default values by customizing the Auto Deploy Helm chart

Auto Build using Heroku buildpacks
Auto Build builds an application using a project's Dockerfile if present, or
otherwise it will use Herokuish
and Heroku buildpacks
to automatically detect and build the application into a Docker image.
Each buildpack requires certain files to be in your project's repository for
Auto Build to successfully build your application. For example, the following
files are required at the root of your application's repository, depending on
the language:

A Pipfile or requirements.txt file for Python projects.
A Gemfile or Gemfile.lock file for Ruby projects.

For the requirements of other languages and frameworks, read the
buildpacks docs.
TIP: Tip:
If Auto Build fails despite the project meeting the buildpack requirements, set
a project variable TRACE=true to enable verbose logging, which may help to
troubleshoot.

Auto Test
Auto Test automatically runs the appropriate tests for your application using
Herokuish and Heroku
buildpacks by analyzing
your project to detect the language and framework. Several languages and
frameworks are detected automatically, but if your language is not detected,
you may succeed with a custom buildpack. Check the
currently supported languages.
Auto Test uses tests you already have in your application. If there are no
tests, it's up to you to add them.

Auto Code Quality (STARTER)

Auto Code Quality uses the
Code Quality image to run
static analysis and other code checks on the current code. The report is
created, and is uploaded as an artifact which you can later download and check
out.
Any differences between the source and target branches are also
shown in the merge request widget.

Auto SAST (ULTIMATE)


Introduced in GitLab Ultimate 10.3.

Static Application Security Testing (SAST) uses the
SAST Docker image to run static
analysis on the current code and checks for potential security issues. The
Auto SAST stage will be skipped on licenses other than Ultimate and requires GitLab Runner 11.5 or above.
Once the report is created, it's uploaded as an artifact which you can later download and
check out.
Any security warnings are also shown in the merge request widget. Read more how
SAST works.

Auto Dependency Scanning (ULTIMATE)


Introduced in GitLab Ultimate 10.7.

Dependency Scanning uses the
Dependency Scanning Docker image
to run analysis on the project dependencies and checks for potential security issues.
The Auto Dependency Scanning stage will be skipped on licenses other than Ultimate
and requires GitLab Runner 11.5 or above.
Once the
report is created, it's uploaded as an artifact which you can later download and
check out.
Any security warnings are also shown in the merge request widget. Read more about
Dependency Scanning.

Auto License Compliance (ULTIMATE)


Introduced in GitLab Ultimate 11.0.

License Compliance uses the
License Compliance Docker image
to search the project dependencies for their license. The Auto License Compliance stage
will be skipped on licenses other than Ultimate.
Once the
report is created, it's uploaded as an artifact which you can later download and
check out.
Any licenses are also shown in the merge request widget. Read more how
License Compliance works.

Auto Container Scanning (ULTIMATE)


Introduced in GitLab 10.4.

Vulnerability Static Analysis for containers uses
Clair to run static analysis on a
Docker image and checks for potential security issues. The Auto Container Scanning stage
will be skipped on licenses other than Ultimate.
Once the report is
created, it's uploaded as an artifact which you can later download and
check out.
Any security warnings are also shown in the merge request widget. Read more how
Container Scanning works.

Auto Review Apps
This is an optional step, since many projects do not have a Kubernetes cluster
available. If the requirements are not met, the job will
silently be skipped.
Review Apps are temporary application environments based on the
branch's code so developers, designers, QA, product managers, and other
reviewers can actually see and interact with code changes as part of the review
process. Auto Review Apps create a Review App for each branch.
Auto Review Apps will deploy your app to your Kubernetes cluster only. When no cluster
is available, no deployment will occur.
The Review App will have a unique URL based on the project ID, the branch or tag
name, and a unique number, combined with the Auto DevOps base domain. For
example, 13083-review-project-branch-123456.example.com. A link to the Review App shows
up in the merge request widget for easy discovery. When the branch or tag is deleted,
for example after the merge request is merged, the Review App will automatically
be deleted.
Review apps are deployed using the
auto-deploy-app chart with
Helm, which can be customized. The app will be deployed into the Kubernetes
namespace
for the environment.
Since GitLab 11.4, a local
Tiller is
used. Previous versions of GitLab had a Tiller installed in the project
namespace.
CAUTION: Caution:
Your apps should not be manipulated outside of Helm (using Kubernetes directly).
This can cause confusion with Helm not detecting the change and subsequent
deploys with Auto DevOps can undo your changes. Also, if you change something
and want to undo it by deploying again, Helm may not detect that anything changed
in the first place, and thus not realize that it needs to re-apply the old config.

Auto DAST (ULTIMATE)


Introduced in GitLab Ultimate 10.4.

This is an optional step, since it requires a review app.
If that requirement is not met, the job will be silently skipped.
Dynamic Application Security Testing (DAST) uses the
popular open source tool OWASP ZAProxy
to perform an analysis on the current code and checks for potential security
issues. The Auto DAST stage will be skipped on licenses other than Ultimate.
Once the report is created, it's uploaded as an artifact which you can
later download and check out.
Any security warnings are also shown in the merge request widget. Read how
DAST works.
On your default branch, DAST scans an app deployed specifically for that purpose.
The app is deleted after DAST has run.
On feature branches, DAST scans the review app.

Overriding the DAST target
To use a custom target instead of the auto-deployed review apps,
set a DAST_WEBSITE environment variable to the URL for DAST to scan.
NOTE: Note:
If DAST Full Scan is enabled, it is strongly advised not
to set DAST_WEBSITE to any staging or production environment. DAST Full Scan
actively attacks the target, which can take down the application and lead to
data loss or corruption.

Disabling Auto DAST
DAST can be disabled:

On all branches by setting the DAST_DISABLED environment variable to "true".
Only on the default branch by setting the DAST_DISABLED_FOR_DEFAULT_BRANCH environment variable to "true".


Auto Browser Performance Testing (PREMIUM)


Introduced in GitLab Premium 10.4.

Auto Browser Performance Testing utilizes the Sitespeed.io container to measure the performance of a web page. A JSON report is created and uploaded as an artifact, which includes the overall performance score for each page. By default, the root page of Review and Production environments will be tested. If you would like to add additional URL's to test, simply add the paths to a file named .gitlab-urls.txt in the root directory, one per line. For example:
/
/features
/direction
Any performance differences between the source and target branches are also
shown in the merge request widget.

Auto Deploy
This is an optional step, since many projects do not have a Kubernetes cluster
available. If the requirements are not met, the job will
silently be skipped.
After a branch or merge request is merged into the project's default branch (usually
master), Auto Deploy deploys the application to a production environment in
the Kubernetes cluster, with a namespace based on the project name and unique
project ID, for example project-4321.
Auto Deploy doesn't include deployments to staging or canary by default, but the
Auto DevOps template contains job definitions for these tasks if you want to
enable them.
You can make use of environment variables to automatically
scale your pod replicas and to apply custom arguments to the Auto DevOps helm upgrade commands. This is an easy way to customize the Auto Deploy Helm chart.
Apps are deployed using the
auto-deploy-app chart with
Helm. The app will be deployed into the Kubernetes
namespace
for the environment.
Since GitLab 11.4, a local
Tiller is
used. Previous versions of GitLab had a Tiller installed in the project
namespace.
CAUTION: Caution:
Your apps should not be manipulated outside of Helm (using Kubernetes directly).
This can cause confusion with Helm not detecting the change and subsequent
deploys with Auto DevOps can undo your changes. Also, if you change something
and want to undo it by deploying again, Helm may not detect that anything changed
in the first place, and thus not realize that it needs to re-apply the old config.

Introduced in GitLab 11.0.

For internal and private projects a GitLab Deploy Token
will be automatically created, when Auto DevOps is enabled and the Auto DevOps settings are saved. This Deploy Token
can be used for permanent access to the registry. When the GitLab Deploy Token has been manually revoked, it won't be automatically created.
If the GitLab Deploy Token cannot be found, CI_REGISTRY_PASSWORD is
used. Note that CI_REGISTRY_PASSWORD is only valid during deployment.
This means that Kubernetes will be able to successfully pull the
container image during deployment but in cases where the image needs to
be pulled again, e.g. after pod eviction, Kubernetes will fail to do so
as it will be attempting to fetch the image using
CI_REGISTRY_PASSWORD.

Migrations

Introduced in GitLab 11.4

Database initialization and migrations for PostgreSQL can be configured to run
within the application pod by setting the project variables DB_INITIALIZE and
DB_MIGRATE respectively.
If present, DB_INITIALIZE will be run as a shell command within an
application pod as a Helm post-install hook. As some applications will
not run without a successful database initialization step, GitLab will
deploy the first release without the application deployment and only the
database initialization step. After the database initialization completes,
GitLab will deploy a second release with the application deployment as
normal.
Note that a post-install hook means that if any deploy succeeds,
DB_INITIALIZE will not be processed thereafter.
If present, DB_MIGRATE will be run as a shell command within an application pod as
a Helm pre-upgrade hook.
For example, in a Rails application in an image built with
Herokuish:


DB_INITIALIZE can be set to RAILS_ENV=production /bin/herokuish procfile exec bin/rails db:setup


DB_MIGRATE can be set to RAILS_ENV=production /bin/herokuish procfile exec bin/rails db:migrate


Unless you have a Dockerfile in your repo, your image is built with
Herokuish, and you must prefix commands run in these images with /bin/herokuish procfile exec to replicate the environment where your application will run.

Workers
Some web applications need to run extra deployments for "worker processes". For
example, it is common in a Rails application to have a separate worker process
to run background tasks like sending emails.
The default Helm chart
used in Auto Deploy has support for running worker
processes.
In order to run a worker, you'll need to ensure that it is able to respond to
the standard health checks, which expect a successful HTTP response on port
5000. For Sidekiq, you could make use of
the sidekiq_alive gem to do this.
In order to work with Sidekiq, you'll also need to ensure your deployments have
access to a Redis instance. Auto DevOps won't deploy this for you so you'll
need to:

Maintain your own Redis instance.
Set a CI variable K8S_SECRET_REDIS_URL, which the URL of this instance to
ensure it's passed into your deployments.

Once you have configured your worker to respond to health checks, you will
need to configure a CI variable HELM_UPGRADE_EXTRA_ARGS with the value
--values helm-values.yaml.
Then you can, for example, run a Sidekiq worker for your Rails application
by adding a file named helm-values.yaml to your repository with the following
content:
workers:
  sidekiq:
    replicaCount: 1
    command:
    - /bin/herokuish
    - procfile
    - exec
    - sidekiq
    preStopCommand:
    - /bin/herokuish
    - procfile
    - exec
    - sidekiqctl
    - quiet
    terminationGracePeriodSeconds: 60

Running commands in the container
Applications built with Auto Build using Herokuish, the default
unless you have a custom Dockerfile, may require
commands to be wrapped as follows:
/bin/herokuish procfile exec $COMMAND
This might be neccessary, for example, when:

Attaching using kubectl exec.
Using GitLab's Web Terminal.

For example, to start a Rails console from the application root directory, run:
/bin/herokuish procfile exec bin/rails c

Auto Monitoring
Once your application is deployed, Auto Monitoring makes it possible to monitor
your application's server and response metrics right out of the box. Auto
Monitoring uses Prometheus to
get system metrics such as CPU and memory usage directly from
Kubernetes,
and response metrics such as HTTP error rates, latency, and throughput from the
NGINX server.
The metrics include:


Response Metrics: latency, throughput, error rate

System Metrics: CPU utilization, memory utilization

To make use of Auto Monitoring:


Install and configure the requirements.

Enable Auto DevOps if you haven't done already.
Finally, go to your project's CI/CD > Pipelines and run a pipeline.
Once the pipeline finishes successfully, open the
monitoring dashboard for a deployed environment
to view the metrics of your deployed application. To view the metrics of the
whole Kubernetes cluster, navigate to Operations > Metrics.



Customizing
While Auto DevOps provides great defaults to get you started, you can customize
almost everything to fit your needs; from custom buildpacks,
to Dockerfiles, Helm charts, or
even copying the complete CI/CD configuration
into your project to enable staging and canary deployments, and more.

Custom buildpacks
If the automatic buildpack detection fails for your project, or if you want to
use a custom buildpack, you can override the buildpack(s) using a project variable
or a .buildpacks file in your project:


Project variable - Create a project variable BUILDPACK_URL with the URL
of the buildpack to use.

.buildpacks file - Add a file in your project's repo called  .buildpacks
and add the URL of the buildpack to use on a line in the file. If you want to
use multiple buildpacks, you can enter them in, one on each line.

CAUTION: Caution:
Using multiple buildpacks isn't yet supported by Auto DevOps.
CAUTION: Caution: When using the .buildpacks file, Auto Test will not work. The buildpack heroku-buildpack-multi (which is used under the hood to parse the .buildpacks file) doesn't provide the necessary commands bin/test-compile and bin/test. Make sure to provide the project variable BUILDPACK_URL instead.

Custom Dockerfile

If your project has a Dockerfile in the root of the project repo, Auto DevOps
will build a Docker image based on the Dockerfile rather than using buildpacks.
This can be much faster and result in smaller images, especially if your
Dockerfile is based on Alpine.

Passing arguments to docker build

Arguments can be passed to the docker build command using the
AUTO_DEVOPS_BUILD_IMAGE_EXTRA_ARGS project variable.
For example, to build a Docker image based on based on the ruby:alpine
instead of the default ruby:latest:


Set AUTO_DEVOPS_BUILD_IMAGE_EXTRA_ARGS to --build-arg=RUBY_VERSION=alpine.


Add the following to a custom Dockerfile:
ARG RUBY_VERSION=latest
FROM ruby:$RUBY_VERSION

# ... put your stuff here


NOTE: Note:
Passing in complex values (newlines and spaces, for example) will likely
cause escaping issues due to the way this argument is used in Auto DevOps.
Consider using Base64 encoding of such values to avoid this problem.
CAUTION: Warning:
Avoid passing secrets as Docker build arguments if possible, as they may be
persisted in your image. See
this discussion for details.

Passing secrets to docker build


Introduced in GitLab 12.3, but available in versions 11.9 and above.

CI environment variables can be passed as build
secrets to the docker build command by listing them comma separated by name in the
AUTO_DEVOPS_BUILD_IMAGE_FORWARDED_CI_VARIABLES variable. For example, in order to forward the variables CI_COMMIT_SHA and CI_ENVIRONMENT_NAME, one would set AUTO_DEVOPS_BUILD_IMAGE_FORWARDED_CI_VARIABLES to CI_COMMIT_SHA,CI_ENVIRONMENT_NAME.
Unlike build arguments, these are not persisted by Docker in the final image
(though you can still persist them yourself, so be careful).
In projects:


Without a Dockerfile, these are available automatically as environment
variables.


With a Dockerfile, the following is required:


Activate the experimental Dockerfile syntax by adding the following
to the top of the file:
# syntax = docker/dockerfile:experimental


To make secrets available in any RUN $COMMAND in the Dockerfile, mount
the secret file and source it prior to running $COMMAND:
RUN --mount=type=secret,id=auto-devops-build-secrets . /run/secrets/auto-devops-build-secrets && $COMMAND




NOTE: Note:
When AUTO_DEVOPS_BUILD_IMAGE_FORWARDED_CI_VARIABLES is set, Auto DevOps
enables the experimental Docker BuildKit
feature to use the --secret flag.

Custom Helm Chart
Auto DevOps uses Helm to deploy your application to Kubernetes.
You can override the Helm chart used by bundling up a chart into your project
repo or by specifying a project variable:


Bundled chart - If your project has a ./chart directory with a Chart.yaml
file in it, Auto DevOps will detect the chart and use it instead of the default
one.
This can be a great way to control exactly how your application is deployed.

Project variable - Create a project variable
AUTO_DEVOPS_CHART with the URL of a custom chart to use or create two project variables AUTO_DEVOPS_CHART_REPOSITORY with the URL of a custom chart repository and AUTO_DEVOPS_CHART with the path to the chart.

You can also make use of the HELM_UPGRADE_EXTRA_ARGS environment variable to override the default values in the values.yaml file in the default Helm chart.
To apply your own values.yaml file to all Helm upgrade commands in Auto Deploy set HELM_UPGRADE_EXTRA_ARGS to --values my-values.yaml.

Custom Helm chart per environment
You can specify the use of a custom Helm chart per environment by scoping the environment variable
to the desired environment. See Limiting environment scopes of variables.

Customizing .gitlab-ci.yml

Auto DevOps is completely customizable because the Auto DevOps template:

Is just an implementation of a .gitlab-ci.yml file.
Uses only features available to any implementation of .gitlab-ci.yml.

If you want to modify the CI/CD pipeline used by Auto DevOps, you can include
the template and customize as
needed. To do this, add a .gitlab-ci.yml file to the root of your repository
containing the following:
include:
  - template: Auto-DevOps.gitlab-ci.yml
Then add any extra changes you want. Your additions will be merged with the
Auto DevOps template using the behaviour described for
include.
It is also possible to copy and paste the contents of the Auto DevOps
template into your project and edit this as needed. You may prefer to do it
that way if you want to specifically remove any part of it.

Using components of Auto DevOps
If you only require a subset of the features offered by Auto DevOps, you can include
individual Auto DevOps jobs into your own .gitlab-ci.yml. Each component job relies
on a stage that should be defined in the .gitlab-ci.yml that includes the template.
For example, to make use of Auto Build, you can add the following to
your .gitlab-ci.yml:
stages:
  - build

include:
  - template: Jobs/Build.gitlab-ci.yml
Consult the Auto DevOps template for information on available jobs.

PostgreSQL database support
In order to support applications that require a database,
PostgreSQL is provisioned by default. The credentials to access
the database are preconfigured, but can be customized by setting the associated
variables. These credentials can be used for defining a
DATABASE_URL of the format:
postgres://user:password@postgres-host:postgres-port/postgres-database

Using external PostgreSQL database providers
While Auto DevOps provides out-of-the-box support for a PostgreSQL container for
production environments, for some use-cases it may not be sufficiently secure or
resilient and you may wish to use an external managed provider for PostgreSQL.
For example, AWS Relational Database Service.
You will need to define environment-scoped variables for POSTGRES_ENABLED and DATABASE_URL in your project's CI/CD settings.
To achieve this:


Disable the built-in PostgreSQL installation for the required environments using
scoped environment variables.
For this use case, it's likely that only production will need to be added to this
list as the builtin PostgreSQL setup for Review Apps and staging will be sufficient
as a high availability setup is not required.



Define the DATABASE_URL CI variable as a scoped environment variable that will be
available to your application. This should be a URL in the following format:
postgres://user:password@postgres-host:postgres-port/postgres-database


You will need to ensure that your Kubernetes cluster has network access to wherever
PostgreSQL is hosted.

Environment variables
The following variables can be used for setting up the Auto DevOps domain,
providing a custom Helm chart, or scaling your application. PostgreSQL can
also be customized, and you can easily use a custom buildpack.

Build and deployment
The following table lists variables related to building and deploying
applications.



Variable
Description




ADDITIONAL_HOSTS
Fully qualified domain names specified as a comma-separated list that are added to the Ingress hosts.


<ENVIRONMENT>_ADDITIONAL_HOSTS
For a specific environment, the fully qualified domain names specified as a comma-separated list that are added to the Ingress hosts. This takes precedence over ADDITIONAL_HOSTS.


AUTO_DEVOPS_BUILD_IMAGE_EXTRA_ARGS
Extra arguments to be passed to the docker build command. Note that using quotes will not prevent word splitting. More details.


AUTO_DEVOPS_BUILD_IMAGE_FORWARDED_CI_VARIABLES
A comma-separated list of CI variable names to be passed to the docker build command as secrets.


AUTO_DEVOPS_CHART
Helm Chart used to deploy your apps. Defaults to the one provided by GitLab.


AUTO_DEVOPS_CHART_REPOSITORY
Helm Chart repository used to search for charts. Defaults to https://charts.gitlab.io.


AUTO_DEVOPS_CHART_REPOSITORY_NAME
From GitLab 11.11, used to set the name of the Helm repository. Defaults to gitlab.


AUTO_DEVOPS_CHART_REPOSITORY_USERNAME
From GitLab 11.11, used to set a username to connect to the Helm repository. Defaults to no credentials. Also set AUTO_DEVOPS_CHART_REPOSITORY_PASSWORD.


AUTO_DEVOPS_CHART_REPOSITORY_PASSWORD
From GitLab 11.11, used to set a password to connect to the Helm repository. Defaults to no credentials. Also set AUTO_DEVOPS_CHART_REPOSITORY_USERNAME.


AUTO_DEVOPS_MODSECURITY_SEC_RULE_ENGINE
From GitLab 12.5, used in combination with Modsecurity feature flag to toggle Modsecurity's SecRuleEngine behavior. Defaults to DetectionOnly.


BUILDPACK_URL
Buildpack's full URL. Can point to either Git repositories or a tarball URL. For Git repositories, it is possible to point to a specific ref. For example https://github.com/heroku/heroku-buildpack-ruby.git#v142.


CANARY_ENABLED
From GitLab 11.0, used to define a deploy policy for canary environments.


CANARY_PRODUCTION_REPLICAS
Number of canary replicas to deploy for Canary Deployments in the production environment. Takes precedence over CANARY_REPLICAS. Defaults to 1.


CANARY_REPLICAS
Number of canary replicas to deploy for Canary Deployments. Defaults to 1.


HELM_RELEASE_NAME
From GitLab 12.1, allows the helm release name to be overridden. Can be used to assign unique release names when deploying multiple projects to a single namespace.


HELM_UPGRADE_EXTRA_ARGS
From GitLab 11.11, allows extra arguments in helm commands when deploying the application. Note that using quotes will not prevent word splitting. Tip: you can use this variable to customize the Auto Deploy Helm chart by applying custom override values with --values my-values.yaml.


INCREMENTAL_ROLLOUT_MODE
From GitLab 11.4, if present, can be used to enable an incremental rollout of your application for the production environment. Set to manual for manual deployment jobs or timed for automatic rollout deployments with a 5 minute delay each one.


K8S_SECRET_*
From GitLab 11.7, any variable prefixed with K8S_SECRET_ will be made available by Auto DevOps as environment variables to the deployed application.


KUBE_INGRESS_BASE_DOMAIN
From GitLab 11.8, can be used to set a domain per cluster. See cluster domains for more information.


PRODUCTION_REPLICAS
Number of replicas to deploy in the production environment. Takes precedence over REPLICAS and defaults to 1. For zero downtime upgrades, set to 2 or greater.


REPLICAS
Number of replicas to deploy. Defaults to 1.


ROLLOUT_RESOURCE_TYPE
From GitLab 11.9, allows specification of the resource type being deployed when using a custom Helm chart. Default value is deployment.


ROLLOUT_STATUS_DISABLED
From GitLab 12.0, used to disable rollout status check because it doesn't support all resource types, for example, cronjob.


STAGING_ENABLED
From GitLab 10.8, used to define a deploy policy for staging and production environments.



TIP: Tip:
Set up the replica variables using a
project variable
and scale your application by just redeploying it!
CAUTION: Caution:
You should not scale your application using Kubernetes directly. This can
cause confusion with Helm not detecting the change, and subsequent deploys with
Auto DevOps can undo your changes.

Database
The following table lists variables related to the database.
| Variable                            | Description                    |
| DB_INITIALIZE                         | From GitLab 11.4, used to specify the command to run to initialize the application's PostgreSQL database. Runs inside the application pod. |
| DB_MIGRATE                            | From GitLab 11.4, used to specify the command to run to migrate the application's PostgreSQL database. Runs inside the application pod. |
| POSTGRES_ENABLED                      | Whether PostgreSQL is enabled. Defaults to "true". Set to false to disable the automatic deployment of PostgreSQL. |
| POSTGRES_USER                         | The PostgreSQL user. Defaults to user. Set it to use a custom username. |
| POSTGRES_PASSWORD                     | The PostgreSQL password. Defaults to testing-password. Set it to use a custom password. |
| POSTGRES_DB                           | The PostgreSQL database name. Defaults to the value of $CI_ENVIRONMENT_SLUG. Set it to use a custom database name. |
| POSTGRES_VERSION                      | Tag for the postgres Docker image to use. Defaults to 9.6.2. |

Security tools
The following table lists variables related to security tools.
| Variable                            | Description                    |
| SAST_CONFIDENCE_LEVEL                 | Minimum confidence level of security issues you want to be reported; 1 for Low, 2 for Medium, 3 for High. Defaults to 3. |

Disable jobs
The following table lists variables used to disable jobs.
| Variable                            | Description                    |
| CODE_QUALITY_DISABLED                 | From GitLab 11.0, used to disable the codequality job. If the variable is present, the job will not be created. |
| CONTAINER_SCANNING_DISABLED           | From GitLab 11.0, used to disable the sast:container job. If the variable is present, the job will not be created. |
| DAST_DISABLED                         | From GitLab 11.0, used to disable the dast job. If the variable is present, the job will not be created. |
| DEPENDENCY_SCANNING_DISABLED          | From GitLab 11.0, used to disable the dependency_scanning job. If the variable is present, the job will not be created. |
| LICENSE_MANAGEMENT_DISABLED           | From GitLab 11.0, used to disable the license_management job. If the variable is present, the job will not be created. |
| PERFORMANCE_DISABLED                  | From GitLab 11.0, used to disable the performance job. If the variable is present, the job will not be created. |
| REVIEW_DISABLED                       | From GitLab 11.0, used to disable the review and the manual review:stop job. If the variable is present, these jobs will not be created. |
| SAST_DISABLED                         | From GitLab 11.0, used to disable the sast job. If the variable is present, the job will not be created. |
| TEST_DISABLED                         | From GitLab 11.0, used to disable the test job. If the variable is present, the job will not be created. |

Application secret variables

Introduced in GitLab 11.7.

Some applications need to define secret variables that are
accessible by the deployed application. Auto DevOps detects variables where the key starts with
K8S_SECRET_ and make these prefixed variables available to the
deployed application, as environment variables.
To configure your application variables:


Go to your project's Settings > CI/CD, then expand the section
called Variables.


Create a CI Variable, ensuring the key is prefixed with
K8S_SECRET_. For example, you can create a variable with key
K8S_SECRET_RAILS_MASTER_KEY.


Run an Auto Devops pipeline either by manually creating a new
pipeline or by pushing a code change to GitLab.


Auto DevOps pipelines will take your application secret variables to
populate a Kubernetes secret. This secret is unique per environment.
When deploying your application, the secret is loaded as environment
variables in the container running the application. Following the
example above, you can see the secret below containing the
RAILS_MASTER_KEY variable.
$ kubectl get secret production-secret -n minimal-ruby-app-54 -o yaml
apiVersion: v1
data:
  RAILS_MASTER_KEY: MTIzNC10ZXN0
kind: Secret
metadata:
  creationTimestamp: 2018-12-20T01:48:26Z
  name: production-secret
  namespace: minimal-ruby-app-54
  resourceVersion: "429422"
  selfLink: /api/v1/namespaces/minimal-ruby-app-54/secrets/production-secret
  uid: 57ac2bfd-03f9-11e9-b812-42010a9400e4
type: Opaque
Environment variables are generally considered immutable in a Kubernetes
pod. Therefore, if you update an application secret without changing any
code then manually create a new pipeline, you will find that any running
application pods will not have the updated secrets. In this case, you
can either push a code update to GitLab to force the Kubernetes
Deployment to recreate pods or manually delete running pods to
cause Kubernetes to create new pods with updated secrets.
NOTE: Note:
Variables with multiline values are not currently supported due to
limitations with the current Auto DevOps scripting environment.

Advanced replica variables setup
Apart from the two replica-related variables for production mentioned above,
you can also use others for different environments.
There's a very specific mapping between Kubernetes' label named track,
GitLab CI/CD environment names, and the replicas environment variable.
The general rule is: TRACK_ENV_REPLICAS. Where:


TRACK: The capitalized value of the track
Kubernetes label
in the Helm Chart app definition. If not set, it will not be taken into account
to the variable name.

ENV: The capitalized environment name of the deploy job that is set in
.gitlab-ci.yml.

That way, you can define your own TRACK_ENV_REPLICAS variables with which
you will be able to scale the pod's replicas easily.
In the example below, the environment's name is qa and it deploys the track
foo which would result in looking for the FOO_QA_REPLICAS environment
variable:
QA testing:
  stage: deploy
  environment:
    name: qa
  script:
  - deploy foo
The track foo being referenced would also need to be defined in the
application's Helm chart, like:
replicaCount: 1
image:
  repository: gitlab.example.com/group/project
  tag: stable
  pullPolicy: Always
  secrets:
    - name: gitlab-registry
application:
  track: foo
  tier: web
service:
  enabled: true
  name: web
  type: ClusterIP
  url: http://my.host.com/
  externalPort: 5000
  internalPort: 5000

Deploy policy for staging and production environments

Introduced
in GitLab 10.8.

TIP: Tip:
You can also set this inside your project's settings.
The normal behavior of Auto DevOps is to use Continuous Deployment, pushing
automatically to the production environment every time a new pipeline is run
on the default branch. However, there are cases where you might want to use a
staging environment and deploy to production manually. For this scenario, the
STAGING_ENABLED environment variable was introduced.
If STAGING_ENABLED is defined in your project (e.g., set STAGING_ENABLED to
1 as a CI/CD variable), then the application will be automatically deployed
to a staging environment, and a  production_manual job will be created for
you when you're ready to manually deploy to production.

Deploy policy for canary environments (PREMIUM)


Introduced
in GitLab 11.0.

A canary environment can be used
before any changes are deployed to production.
If CANARY_ENABLED is defined in your project (e.g., set CANARY_ENABLED to
1 as a CI/CD variable) then two manual jobs will be created:


canary which will deploy the application to the canary environment

production_manual which is to be used by you when you're ready to manually
deploy to production.


Incremental rollout to production (PREMIUM)


Introduced in GitLab 10.8.

TIP: Tip:
You can also set this inside your project's settings.
When you have a new version of your app to deploy in production, you may want
to use an incremental rollout to replace just a few pods with the latest code.
This will allow you to first check how the app is behaving, and later manually
increasing the rollout up to 100%.
If INCREMENTAL_ROLLOUT_MODE is set to manual in your project, then instead
of the standard production job, 4 different
manual jobs
will be created:

rollout 10%
rollout 25%
rollout 50%
rollout 100%

The percentage is based on the REPLICAS variable and defines the number of
pods you want to have for your deployment. If you say 10, and then you run
the 10% rollout job, there will be 1 new pod + 9 old ones.
To start a job, click on the play icon next to the job's name. You are not
required to go from 10% to 100%, you can jump to whatever job you want.
You can also scale down by running a lower percentage job, just before hitting
100%. Once you get to 100%, you cannot scale down, and you'd have to roll
back by redeploying the old version using the
rollback button in the
environment page.
Below, you can see how the pipeline will look if the rollout or staging
variables are defined.
Without INCREMENTAL_ROLLOUT_MODE and without STAGING_ENABLED:

Without INCREMENTAL_ROLLOUT_MODE and with STAGING_ENABLED:

With INCREMENTAL_ROLLOUT_MODE set to manual and without STAGING_ENABLED:

With INCREMENTAL_ROLLOUT_MODE set to manual and with STAGING_ENABLED

CAUTION: Caution:
Before GitLab 11.4 this feature was enabled by the presence of the
INCREMENTAL_ROLLOUT_ENABLED environment variable.
This configuration is deprecated and will be removed in the future.

Timed incremental rollout to production (PREMIUM)


Introduced in GitLab 11.4.

TIP: Tip:
You can also set this inside your project's settings.
This configuration is based on
incremental rollout to production.
Everything behaves the same way, except:

It's enabled by setting the INCREMENTAL_ROLLOUT_MODE variable to timed.
Instead of the standard production job, the following jobs are created with a 5 minute delay between each :

timed rollout 10%
timed rollout 25%
timed rollout 50%
timed rollout 100%




Currently supported languages
Note that not all buildpacks support Auto Test yet, as it's a relatively new
enhancement. All of Heroku's officially supported
languages
support it, and some third-party buildpacks as well e.g., Go, Node, Java, PHP,
Python, Ruby, Gradle, Scala, and Elixir all support Auto Test, but notably the
multi-buildpack does not.
As of GitLab 10.0, the supported buildpacks are:
- heroku-buildpack-multi     v1.0.0
- heroku-buildpack-ruby      v168
- heroku-buildpack-nodejs    v99
- heroku-buildpack-clojure   v77
- heroku-buildpack-python    v99
- heroku-buildpack-java      v53
- heroku-buildpack-gradle    v23
- heroku-buildpack-scala     v78
- heroku-buildpack-play      v26
- heroku-buildpack-php       v122
- heroku-buildpack-go        v72
- heroku-buildpack-erlang    fa17af9
- buildpack-nginx            v8

Limitations
The following restrictions apply.

Private registry support
There is no documented way of using private container registry with Auto DevOps.
We strongly advise using GitLab Container Registry with Auto DevOps in order to
simplify configuration and prevent any unforeseen issues.

Installing Helm behind a proxy
GitLab does not yet support installing Helm as a GitLab-managed App when
behind a proxy. Users who wish to do so must inject their proxy settings
into the installation pods at runtime, for example by using a
PodPreset:
apiVersion: settings.k8s.io/v1alpha1
kind: PodPreset
metadata:
  name: gitlab-managed-apps-default-proxy
  namespace: gitlab-managed-apps
spec:
   env:
    - name: http_proxy
      value: "PUT_YOUR_HTTP_PROXY_HERE"
    - name: https_proxy
      value: "PUT_YOUR_HTTPS_PROXY_HERE"

Troubleshooting

Auto Build and Auto Test may fail in detecting your language/framework. There
may be no buildpack for your application, or your application may be missing the
key files the buildpack is looking for. For example, for ruby apps, you must
have a Gemfile to be properly detected, even though it is possible to write a
Ruby app without a Gemfile. Try specifying a custom
buildpack.
Auto Test may fail because of a mismatch between testing frameworks. In this
case, you may need to customize your .gitlab-ci.yml with your test commands.
Auto Deploy will fail if GitLab can not create a Kubernetes namespace and
service account for your project. For help debugging this issue, see
Troubleshooting failed deployment jobs.


Disable the banner instance wide
If an administrator would like to disable the banners on an instance level, this
feature can be disabled either through the console:
sudo gitlab-rails console
Then run:
Feature.get(:auto_devops_banner_disabled).enable
Or through the HTTP API with an admin access token:
curl --data "value=true" --header "PRIVATE-TOKEN: personal_access_token" https://gitlab.example.com/api/v4/features/auto_devops_banner_disabled

Development guides
Development guide for Auto DevOps
