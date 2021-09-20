resource "aws_cloudwatch_event_rule" "cost_reporting_trigger" {
  name                = "cost_reporting_trigger"
  description         = "triggers once a month for cost report"
  schedule_expression = "cron(0 8 1 * ? *)"
  tags = {
    Owner = "Emmanuel Pius-Ogiji"
  }
}

resource "aws_cloudwatch_event_target" "run_cost_report" {
  arn       = aws_lambda_function.cost_reporting.arn
  target_id = aws_lambda_function.cost_reporting.id
  rule      = aws_cloudwatch_event_rule.cost_reporting_trigger.name
  input_transformer {
    input_paths = {
      time = "$.time"
    }
    input_template = "\"Cost reporting run triggered at <time>\""
  }
}

resource "aws_lambda_permission" "allow_cloudwatch_trigger_cost_report" {
  statement_id  = "AllowExecutionFromCloudWatch"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.cost_reporting.function_name
  principal     = "events.amazonaws.com"
  source_arn    = aws_cloudwatch_event_rule.cost_reporting_trigger.arn
}