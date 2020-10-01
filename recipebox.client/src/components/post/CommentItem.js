import React from 'react';
import Moment from 'react-moment';
import moment from 'moment';
import PropTypes from 'prop-types';
import { Button, Comment, Form, Header } from 'semantic-ui-react';

const CommentItem = ({ postId, comment: { text, created, commentId, commenterId } }) => {
	const dateFormatted = moment(created).format('YYYYMMDD');
	const todaysDate = new Date().toISOString().slice(0, 10).replace(/-/g, '');
	console.log(dateFormatted);
	console.log(todaysDate);
	return (
		<Comment.Group>
			<Comment>
				<Comment.Avatar src='https://react.semantic-ui.com/images/avatar/small/matt.jpg' />
				<Comment.Content>
					<Comment.Author as='a'>Matt</Comment.Author>
					<Comment.Metadata>
						<div>
							{dateFormatted === todaysDate ? (
								<h4>Today</h4>
							) : (
								<h4>
									<Moment format='DD/MM/YYYY'>{created}</Moment>
								</h4>
							)}
						</div>
					</Comment.Metadata>
					<Comment.Text>How artistic!</Comment.Text>
					<Comment.Actions>
						<Comment.Action>Reply</Comment.Action>
					</Comment.Actions>
				</Comment.Content>
			</Comment>
		</Comment.Group>
	);
};

CommentItem.propTypes = {};

export default CommentItem;
