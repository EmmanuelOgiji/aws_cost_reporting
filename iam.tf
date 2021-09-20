resource "aws_iam_role" "cost_reporting_role" {
  name                  = "aws_cost_reporting_role"
  force_detach_policies = true
  tags = {
    Owner = "Emmanuel Pius-Ogiji"
  }

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

resource "aws_iam_role_policy" "cost_reporting_policy" {
  name   = "cost_reporting_policy"
  role   = aws_iam_role.cost_reporting_role.id
  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "CostAndUsage",
            "Effect": "Allow",
            "Action": "ce:GetCostAndUsage",
            "Resource": "*"
        },
        {
            "Sid": "SNSAccess",
            "Effect": "Allow",
            "Action": "sns:Publish",
            "Resource": "${aws_sns_topic.cost_reporting.arn}"
        },
        {
            "Sid": "SESAccess",
            "Effect":"Allow",
            "Action":[
              "ses:SendEmail",
              "ses:SendRawEmail"
            ],
            "Resource":"*",
            "Condition":{
              "ForAllValues:StringLike":{
                "ses:Recipients":[
                  "*@ecs.co.uk"
                ],
                "ses:FromAddress":[
                  "*@ecs.co.uk"
                ]
              }
            }
        },
        {
            "Sid": "LogPermissions",
            "Effect": "Allow",
            "Action": [
                "logs:CreateLogStream",
                "logs:PutLogEvents",
                "logs:CreateLogGroup"
            ],
            "Resource": "arn:aws:logs:*:${data.aws_caller_identity.account.account_id}:*"
        }
    ]
}
EOF
}