data "aws_s3_bucket_object" "cost_reporting_code" {
  bucket = var.cost_reporting_bucket_id
  key    = var.compiled_code_s3_key
}

resource "aws_lambda_function" "cost_reporting" {
  s3_bucket         = var.cost_reporting_bucket_id
  s3_key            = data.aws_s3_bucket_object.cost_reporting_code.key
  s3_object_version = data.aws_s3_bucket_object.cost_reporting_code.version_id
  function_name     = "aws_cost_reporting"
  role              = aws_iam_role.cost_reporting_role.arn
  handler           = "CostReporting::CostReporting.CostReporter::FunctionHandler"
  runtime           = "dotnetcore3.1"
  timeout           = 30
  tags = {
    Owner = "Emmanuel Pius-Ogiji"
  }
  environment {
    variables = {
      sns_topic_arn      = aws_sns_topic.cost_reporting.arn,
      sender_address     = var.ses_sender_email,
      receiver_addresses = var.ses_receiver_emails
    }
  }
}
