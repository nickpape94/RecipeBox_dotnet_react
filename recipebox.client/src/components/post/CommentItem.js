import React from 'react';
import Moment from 'react-moment';
import { Link } from 'react-router-dom';
import moment from 'moment';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { deleteComment } from '../../actions/post';

const CommentItem = ({
	deleteComment,
	comment: { text, created, author, userPhotoUrl, commenterId, commentId },
	auth: { user }
}) => {
	const dateFormatted = moment(created).format('YYYYMMDD');
	const todaysDate = new Date().toISOString().slice(0, 10).replace(/-/g, '');

	console.log(commenterId);
	console.log(user.id);
	return (
		<div className='comment bg-white p-1 my-1'>
			<div>
				<Link
					to={{
						pathname: `/users/${commenterId}`,
						state: {
							fromPost: true
						}
					}}
				>
					<img className='round-img' src={userPhotoUrl} alt='' />
					<h4>{author}</h4>
				</Link>
			</div>
			<div>
				<p className='my-1'>{text}</p>
				<p className='post-date'>
					{dateFormatted === todaysDate ? (
						<h4>Today</h4>
					) : (
						<h4>
							<Moment format='DD/MM/YYYY'>{created}</Moment>
						</h4>
					)}
				</p>
				{/* {!auth.loading &&
				user === auth.user._id && (
					<button onClick={(e) => deleteComment(postId, _id)} type='button' className='btn btn-danger'>
						<i className='fas fa-times' />
					</button>
				)} */}
			</div>
			{commenterId === user.id && (
				<button onClick={() => deleteComment(user.id, commentId)}>
					<i className='fas fa-trash-alt' />
				</button>
			)}
		</div>
	);
};

CommentItem.propTypes = {
	user: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired,
	deleteComment: PropTypes.func.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, { deleteComment })(CommentItem);
