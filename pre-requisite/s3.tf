resource "aws_s3_bucket" "cost_reporting_repo" {
  bucket = "${data.aws_iam_account_alias.account.account_alias}-cost-reporting-bucket"
  acl    = "private"

  tags = {
    Owner = "Emmanuel Pius-Ogiji"
  }
}

output "deployment_bucket_id" {
  value = aws_s3_bucket.cost_reporting_repo.id
}

variable "region" {
  type        = string
  description = "The region for deployment"
  default     = "eu-west-1"
}

variable "access_key" {
  type        = string
  description = "The AWS access key to access the AWS account"
  default     = ""
}

variable "secret_key" {
  type        = string
  description = "The AWS secret key to access the AWS account"
  default     = ""
}
