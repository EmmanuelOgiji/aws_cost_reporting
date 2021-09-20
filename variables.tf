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


variable "cost_reporting_bucket_id" {
  type        = string
  description = "S3 key for built code published to S3"
  default     = ""
}


variable "compiled_code_s3_key" {
  type        = string
  description = "S3 key for built code published to S3"
  default     = "CostReporting.zip"
}

variable "ses_sender_email" {
  type        = string
  description = "email address from which cost report SES emails are sent out"
  default     = "emmanuel.ogiji@ecs.co.uk"
}

variable "ses_receiver_emails" {
  type        = string
  description = "string comma-separated email addresss to receive cost report SES emails"
  default     = "emmanuel.ogiji@ecs.co.uk,alastair.hill@ecs.co.uk,mark.capaldi@ecs.co.uk"
}
