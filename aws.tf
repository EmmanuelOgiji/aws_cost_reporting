terraform {
  required_version = "~> 0.14.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.23.0"
    }
  }
  backend "s3" {
    bucket = "ecsd-poc-cost-reporting"
    key    = "tfstate/terraform.tfstate"
    region = "eu-west-1"
  }
}

provider "aws" {
  region     = var.region
  access_key = var.access_key
  secret_key = var.secret_key
}

data "aws_caller_identity" "account" {}
data "aws_iam_account_alias" "account" {}
